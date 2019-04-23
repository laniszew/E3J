using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Driver.Exceptions;
using System.Text.RegularExpressions;
using System.Threading;

namespace Driver
{
    public class ProgramService
    {
        private readonly E3JManipulator manipulator;

        #region Enums

        public enum EventType
        {
            [Description("Program Downloaded")]
            PROGRAM_DOWNLOADED,

            [Description("Program Uploaded")]
            PROGRAM_UPLOADED,

            [Description("Line Uploaded")]
            LINE_UPLOADED
        }

        #endregion

        #region Events

        public delegate void StepUpdateHandler(object sender, NotificationEventArgs e);

        public event StepUpdateHandler StepUpdate;

        #endregion

        public ProgramService(E3JManipulator manipulator)
        {
            this.manipulator = manipulator;
        }

        public async Task StopProgram()
        {
            await StopProgram(default(CancellationToken));
        }

        public async Task StopProgram(CancellationToken cancellationToken)
        {
            if (!manipulator.Connected) return;
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                manipulator.Halt();
                await Task.Delay(200, cancellationToken);
                manipulator.Reset(0);
            }
            catch(OperationCanceledException)
            {
                // Cancellation does not require handling as method returns immediately after catch clause
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
        }

        public async Task RunProgram(RemoteProgram remoteProgram)
        {
            await RunProgram(remoteProgram, default(CancellationToken));
        }

        public async Task RunProgram(RemoteProgram remoteProgram, CancellationToken cancellationToken)
        {
            if (!manipulator.Connected) return;
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                manipulator.Number(remoteProgram.Name);
                await Task.Delay(1000, cancellationToken);
                manipulator.Run();
            }
            catch (OperationCanceledException)
            {
                // Cancellation does not require handling as method returns immediately after catch clause 
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
        }

        public async Task<Program> DownloadProgram(RemoteProgram remoteProgram)
        {
            return await DownloadProgram(remoteProgram, default(CancellationToken));
        }

        /// <summary>
        /// Uploads program from manipulator to PC memory
        /// </summary>
        /// <param name="remoteProgram">Program downloaded from manipulator</param>
        /// <returns>Requested program or null when program with given name does not exist</returns>
        public async Task<Program> DownloadProgram(RemoteProgram remoteProgram, CancellationToken cancellationToken)
        {
            manipulator.Number(remoteProgram.Name);
            await Task.Delay(1000, cancellationToken);
            var errorCode = await manipulator.ErrorRead();

            if (errorCode != 0)
            {
                throw new AlarmException(errorCode);
            }

            var content = string.Empty;
            for (uint i = 1; ; i++)
            {
                var line = await manipulator.StepRead(i);
                if (line.Equals("\r"))
                    break;
                content += line + "\n";

                try
                {
                    cancellationToken.ThrowIfCancellationRequested();
                }
                catch(OperationCanceledException)
                {
                    break;
                }

            }
            return Program.CreateFromRemoteProgram(remoteProgram, content);
        }

        /// <summary>
        /// Receives all programs downloaded from manipulator
        /// </summary>
        /// <returns></returns>
        public async Task<List<Program>> DownloadPrograms(List<RemoteProgram> remotePrograms)
        {
            return await DownloadPrograms(remotePrograms, default(CancellationToken));
        }

        /// <summary>
        /// Receives all programs downloaded from manipulator
        /// </summary>
        /// <returns></returns>
        public async Task<List<Program>> DownloadPrograms(List<RemoteProgram> remotePrograms, CancellationToken cancellationToken)
        {
            var programs = new List<Program>();
            for (var i = 0; i < remotePrograms.Count; i++)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();
                }
                catch(OperationCanceledException)
                {
                    break;
                }

                programs.Add(await DownloadProgram(remotePrograms[i], cancellationToken));
                StepUpdate?.Invoke(this, new NotificationEventArgs("Downloading programs", i + 1,
                    remotePrograms.Count, EventType.PROGRAM_DOWNLOADED));
            }
            return programs;
        }

        /// <summary>
        /// Sends program to manipulator
        /// </summary>
        /// <param name="program">Program to upload to manipulator</param>
        public async Task UploadProgram(Program program)
        {
            await UploadProgram(program, default(CancellationToken));
        }

        /// <summary>
        /// Sends program to manipulator
        /// </summary>
        /// <param name="program">Program to be uploaded to manipulator</param>
        /// <param name="cancellationToken">Defines task cancellation operation</param>
        public async Task UploadProgram(Program program, CancellationToken cancellationToken)
        {
            if (!manipulator.Connected) return;
            try
            {
                manipulator.Number(program.Name);
                await Task.Delay(1000, cancellationToken);
                manipulator.New();
                await Task.Delay(1000, cancellationToken);

                var lines = program.GetLines();

                for (var i = 0; i < lines.Count; i++)
                {
                    await Task.Delay(500, cancellationToken);
                    cancellationToken.ThrowIfCancellationRequested();

                    manipulator.SendCustom(lines[i]);
                    StepUpdate?.Invoke(this, new NotificationEventArgs("Uploading program", i + 1,
                        lines.Count, EventType.LINE_UPLOADED));
                }
            }
            catch(OperationCanceledException)
            {
                // Cancellation does not require handling as method returns immediately after catch clause
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
        }

        public async Task DeleteProgram(string programName)
        {
            await DeleteProgram(programName, default(CancellationToken));
        }

        /// <summary>
        /// [Deprecated] Deletes program from manipulator memory
        /// </summary>
        /// <param name="programName">Deleted program name</param>
        public async Task<bool> DeleteProgram(string programName, CancellationToken cancellationToken)
        {
            if (!manipulator.Connected) return false;
            try
            {
                manipulator.Number(programName);
                await Task.Delay(1000, cancellationToken);
                manipulator.New();
                await Task.Delay(1000, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                return false;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
            return true;
        }

        public async Task<List<RemoteProgram>> ReadProgramInfo()
        {
            return await ReadProgramInfo(default(CancellationToken));
        }

        public async Task<List<RemoteProgram>> ReadProgramInfo(CancellationToken cancellationToken)
        {
            var remoteProgramList = new List<RemoteProgram>();

            // Decode data
            for (var i = 1; ; i++)
            {
                manipulator.SendCustom(i == 1 ? "EXE0, \"Fd<*\"" : $"EXE0, \"Fd{i}\"");
                
                await manipulator.Port.WaitForMessageAsync();
                var QoK = manipulator.Port.Read();
                if (QoK.Equals("QoK\r") || Regex.IsMatch(QoK, @"^QoK\s*$"))
                    break;

                try
                {
                    cancellationToken.ThrowIfCancellationRequested();
                }
                catch(OperationCanceledException)
                {
                    break;
                }

                var remoteProgram = RemoteProgram.Create(QoK);
                if (remoteProgram != null)
                    remoteProgramList.Add(remoteProgram);
            }
            return remoteProgramList;
        }
    }
}