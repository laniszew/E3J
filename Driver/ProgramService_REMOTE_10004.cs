using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Driver.Exceptions;

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

        public async void StopProgram()
        {
            if (!manipulator.Connected) return;
            try
            {
                manipulator.Halt();
                await Task.Delay(200);
                manipulator.Reset(0);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
        }

        public async void RunProgram(RemoteProgram remoteProgram)
        {
            if (!manipulator.Connected) return;
            try
            {
                manipulator.Number(remoteProgram.Name);
                await Task.Delay(1000);
                manipulator.Run();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Uploads program from manipulator to PC memory
        /// </summary>
        /// <param name="remoteProgram">Program downloaded from manipulator</param>
        /// <returns>Requested program or null when program with given name does not exist</returns>
        public async Task<Program> DownloadProgram(RemoteProgram remoteProgram)
        {
            manipulator.Number(remoteProgram.Name);
            await Task.Delay(1000);
            var errorCode = await manipulator.ErrorRead();

            if (errorCode != 0)
            {
                throw new AlarmException(errorCode);
            }

            var content = string.Empty;
            for (uint i = 1;; i++)
            {
                var line = await manipulator.StepRead(i);
                if (line.Equals("\r"))
                    break;
                content += line + "\n";
            }
            return Program.CreateFromRemoteProgram(remoteProgram, content);
        }

        /// <summary>
        /// Receives all programs downloaded from manipulator
        /// </summary>
        /// <returns></returns>
        public async Task<List<Program>> DownloadPrograms(List<RemoteProgram> remotePrograms)
        {
            var programs = new List<Program>();
            for(var i = 0; i < remotePrograms.Count; i++)
            {
                programs.Add(await DownloadProgram(remotePrograms[i]));
                StepUpdate?.Invoke(this, new NotificationEventArgs("Downloading programs", i+1, 
                    remotePrograms.Count, EventType.PROGRAM_DOWNLOADED));
            }
            return programs;
        }

        /// <summary>
        /// Sends program to manipulator
        /// </summary>
        /// <param name="program"></param>
        public async void UploadProgram(Program program)
        {
            if (!manipulator.Connected) return;
            try
            {
                manipulator.Number(program.Name);
                await Task.Delay(1000);
                manipulator.New();
                await Task.Delay(1000);

                var lines = program.GetLines();

                for (var i = 0; i < lines.Count; i++)
                {
                    await Task.Delay(500);
                    manipulator.SendCustom(lines[i]);
                    StepUpdate?.Invoke(this, new NotificationEventArgs("Uploading program", i+1, 
                        lines.Count, EventType.LINE_UPLOADED));
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// [Deprecated] Deletes program from manipulator memory
        /// </summary>
        /// <param name="programName">Deleted program name</param>
        public void DeleteProgram(string programName)
        {
            throw new NotImplementedException();
        }

        public async Task<List<RemoteProgram>> ReadProgramInfo()
        {
            var remoteProgramList = new List<RemoteProgram>();

            // Decode data
            for (var i = 1; ; i++)
            {
                manipulator.SendCustom(i == 1 ? "EXE0, \"Fd<*\"" : $"EXE0, \"Fd{i}\"");

                await manipulator.Port.WaitForMessageAsync();
                var QoK = manipulator.Port.Read();
                if (QoK.Equals("QoK\r"))
                    break;

                var remoteProgram = RemoteProgram.Create(QoK);
                if (remoteProgram != null)
                    remoteProgramList.Add(remoteProgram);
            }
            return remoteProgramList;
        }
    }
}
