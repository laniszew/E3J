using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Driver
{
    public class E3JManipulator
    {
        #region Enums
        public enum GrabE { Closed, Open };

        public enum OriginMethodE
        {
            Mechanical_Stopper,
            Jig,
            User_Defined
        };

        public enum InterpolationE
        {
            Joint,
            Linear,
            Circular,
        };

        public enum JointE
        {
            waist = 1,
            shoulder,
            elbow,
            pitch,
            roll
        };

        public enum ContentE
        {
            Counter,
            Position,
            CharacterString
        };

        public enum SignE
        {
            Plus,
            Minus
        };
        #endregion

        #region Fields and Properties
        public SerialCOM Port { get; }
        public bool Connected => Port.Opened;
        #endregion

        public E3JManipulator(DriverSettings settings)
        {
            Port = new SerialCOM(settings);
            Port.DataReceived += Port_DataReceived;
        }

        public void Connect(string portName)
        {
            Port.OpenPort(portName);
        }

        public void Disconnect()
        {
            Port.ClosePort();
        }


        public void SendCustom(string expression)
        {
            Port.Write($"{expression}");
        }


        /// <summary>
        /// ANDs the specified value with the internal register, then stoers the result to the internal register.
        /// </summary>
        /// <param name="operationData"></param>
        public void And(double operationData)
        {
            Port.Write($"AN {operationData}");
        }

        /// <summary>
        /// The internal register value is set in the counter with the specified number. The character string register details
        /// are set in the character string with the specified value.
        /// </summary>
        /// <param name="counternNumber">Specify counter number in numeric value. [1..99]</param>
        public void CounterLoad(double counternNumber)
        {
            Port.Write($"CL {counternNumber}");
        }

        /// <summary>
        /// The internal register value is set in the counter with the specified number. The character string register details
        /// are set in the character string with the specified value.
        /// </summary>
        /// <param name="characterStringNumber">Specify character string number in numerical value which "$" is added to the head. [1..99]</param>
        public void CounterLoad(uint characterStringNumber)
        {
            Port.Write($"CL {characterStringNumber}");
        }

        /// <summary>
        /// The value of the specified counter is set in the internal register. The details of the specified string are set
        /// in the character string register.
        /// </summary>
        /// <param name="counternNumber">Specify counter number in numeric value. [1..99]</param>
        public void CompareCounter(double counterNumber)
        {
            Port.Write($"CP {counterNumber}");
        }

        /// <summary>
        /// The value of the specified counter is set in the internal register. The details of the specified string are set
        /// in the character string register.
        /// </summary>
        /// <param name="characterStringNumber">Specify character string number in numerical value which "$" is added to the head. [1..99]</param>
        public void CompareCounter(uint characterStringNumber)
        {
            Port.Write($"CP {characterStringNumber}");
        }

        /// <summary>
        /// The details of the specified counter or character string are read out.
        /// </summary>
        /// <param name="counternNumber">Specify counter number in numeric value. [1..99]</param>
        public void CounterRead(double counternNumber)
        {
            Port.Write($"CR {counternNumber}");
        }

        /// <summary>
        /// The details of the specified counter or character string are read out.
        /// </summary>
        /// <param name="characterStringNumber">Specify character string number in numerical value which "$" is added to the head. [1..99]</param>
        public void CounterRead(uint characterStringNumber)
        {
            Port.Write($"CR ${characterStringNumber}");
        }

        /// <summary>
        /// Disables an interrupt of the specified bit through the external input port. Check application note before use!! page: 3-11
        /// </summary>
        /// <param name="inputBitNumber">Specify the bit number to be disabled</param>
        public void DisableAct(double inputBitNumber)
        {
            Port.Write($"DC {inputBitNumber}");
        }

        /// <summary>
        /// Substracts 1 from the value in the specified counter.
        /// </summary>
        /// <param name="counternNumber">Specify counter number in numeric value. [1..99]</param>
        public void DecrementCounter(double counternNumber)
        {
            Port.Write($"DC {counternNumber}");
        }

        /// <summary>
        /// Rotates the specified joint by the specified angle from the current position. (Joint interpolation)
        /// </summary>
        /// <param name="jointNumber">Specify joint number that you want to move.[1..5]
        /// 1: waist joint;  2: shoulder joint;  3: elbow joint;  4: pitch joint;  5: roll joint</param>
        /// <param name="turningAngle">Specify joint angle that you want to turn.</param>
        public void DrawJoint(JointE jointNumber, double turningAngle)
        {
            Port.Write($"DJ {(int)jointNumber}, {turningAngle}");
        }

        /// <summary>
        /// Deletes commands of the specified line or step in the program.
        /// </summary>
        /// <param name="lineNumberA">Specify the top line number that you want to delete in the program.</param>
        public void DeleteLine(uint lineNumberA)
        {
            Port.Write($"DL {lineNumberA}");
        }

        /// <summary>
        /// Moves the robot to a predefined position with a position number smaller than the current one. (Joint interpolation)
        /// </summary>
        public void DecrementPosition()
        {
            Port.Write($"DP");
        }

        /// <summary>
        /// Reads the values of the internal register, hand check state, and general output state.
        /// </summary>
        public void DataRead()
        {
            Port.Write($"DR");
        }

        /// <summary>
        /// Moves the end of the hand to a position away from the current position by the distance specified in X, Y, and Z
        /// directions. (Joint interpolation)
        /// </summary>
        /// <param name="travelDistanceInX">Specify the amount that you want to move in X direction from the current position.</param>
        /// <param name="travelDistanceInY">Specify the amount that you want to move in Y direction from the current position.</param>
        /// <param name="travelDistanceInZ">Specify the amount that you want to move in Z direction from the current position.</param>
        public void DrawStraight(double travelDistanceInX, double travelDistanceInY, double travelDistanceInZ)
        {
            Port.Write($"DS {travelDistanceInX}, {travelDistanceInY}, {travelDistanceInZ}");
        }

        /// <summary>
        /// Moves the end of the hand to a position away from the current position by the distance specified in X, Y and Z directions. (Joint interpolation)
        /// </summary>
        /// <param name="travelDistanceInX">Specify the amount that you want to move in X direction from the current position.</param>
        /// <param name="travelDistanceInY">Specify the amount that you want to move in Y direction from the current position.</param>
        /// <param name="travelDistanceInZ">Specify the amount that you want to move in Z direction from the current position.</param>
        public void Draw(double travelDistanceInX = 0, double travelDistanceInY = 0, double travelDistanceInZ = 0)
        {
            Port.Write($"DW {travelDistanceInX},{travelDistanceInY},{travelDistanceInZ}");
        }

        /// <summary>
        /// Enables the interrupt motion by the specified bit of the external input signal. Ch
        /// </summary>
        /// <param name="sign">+: if the inputBitNumber of the external input port turns ON, the program jumps to the lineNumber;
        /// -: if the inputBitNumber of the external input port turns OFF, the program jumps to the lineNumber</param>
        /// <param name="inputBitNumber">specify the bit number of external input signal that you want to assign for interrupt signal. [0..32767]</param>
        /// <param name="lineNumber"></param>
        public void EnableAct(SignE sign, double inputBitNumber, double lineNumber)
        {
            var strSign = sign == SignE.Plus ? "+" : "-";
            Port.Write($"EA {strSign},{inputBitNumber},{lineNumber}");
        }

        /// <summary>
        /// Ends the program.
        /// </summary>
        public void End()
        {
            Port.Write($"ED");
        }

        /// <summary>
        /// This compares the value of the internal register with a specified value. If they are the same, the program will jump
        /// to specified line number. The character string register and the details of the specified character string are compared,
        /// and if the values are the same, the program will jump to the specified line number.
        /// </summary>
        /// <param name="comparedValue">Specify the value that the internal register compares contents with. [-32768..32767]</param>
        /// <param name="branchingLineNumber">Specify the line number to which the program jumps when the comparison result is equal. [1...9999]</param>
        public void Equal(double comparedValue, uint branchingLineNumber)
        {
            Port.Write($"EQ {comparedValue},{branchingLineNumber}");
        }

        /// <summary>
        /// Reads the current error status and alarm history contents.
        /// </summary>
        public async Task<int> ErrorRead()
        {
            Port.Write("ER");
            await Port.WaitForMessageAsync();
            var code = Port.Read();
            return string.IsNullOrEmpty(code) ? Convert.ToInt32(code) : 0;
        }

        /// <summary>
        /// Close the grip of hand.
        /// </summary>
        public void GrabClose()
        {
            Port.Write("GC");
        }

        /// <summary>
        /// Opens the grip of the hand.
        /// </summary>
        public void GrabOpen()
        {
            Port.Write("GO");
        }
        /// <summary>
        /// Defines the gripping force to be applied the motor-operated hand is closed and opened.
        /// </summary>
        /// <param name="startingGrippingForce">Specify necessary gripping force in integer value to activate hand open or close.</param>
        /// <param name="retainedGrippingForce">Specify necessary gripping force in integer value to maintain hand open or close.</param>
        /// <param name="startingGrippingForceRetentionTime">Specify time continuing starting gripping force in integer value.</param>
        public void GripPressure(double startingGrippingForce, double retainedGrippingForce,
                                 double startingGrippingForceRetentionTime)
        {
            Port.Write($"GP {startingGrippingForce}, {retainedGrippingForce}, {startingGrippingForceRetentionTime}");
        }

        /// <summary>
        /// Carries out subroutine beginning with the specified line number.
        /// </summary>
        /// <param name="lineNumber">Specify line number of subroutine in integer value.</param>
        public void GoSub(uint lineNumber)
        {
            Port.Write($"GS {lineNumber}");
        }

        /// <summary>
        /// Jumps to the specified line number unconditionally.
        /// </summary>
        /// <param name="lineNumber"></param>
        public void GoTo(uint lineNumber)
        {
            Port.Write($"GT {lineNumber}");
        }

        /// <summary>
        /// Defines the current coordinates as the specified position.
        /// </summary>
        /// <param name="positionNumber">Specify the position number to be registered [0, 999]. Registers the current position to the user-defined origin in case of zero.</param>
        public void Here(uint positionNumber)
        {
            Port.Write($"HE {positionNumber}");
        }

        /// <summary>
        /// Interrupts the motion of the robot and the operation of the program.
        /// </summary>
        public void Halt()
        {
            Port.Write("HLT");
        }

        /// <summary>
        /// Defines the current location and the attitude as origin point.
        /// </summary>
        /// <param name="originSettingAproach">Specify the method to set origin in integer value:  
        /// 0: Mechanical stopper origin;
        /// 1: Jig origin;
        /// 2: User-defined origin</param>
        public void Home(OriginMethodE originSettingAproach)
        {
            Port.Write($"HO {(int)originSettingAproach}");
        }

        /// <summary>
        /// Moves the robot to a predifined position with a position number greater than the current one.
        /// </summary>
        public void IncrementPosition()
        {
            Port.Write("IP");
        }

        /// <summary>
        /// Adds 1 to the value of the specified counter.
        /// </summary>
        /// <param name="counterNumber">Specify counter number in numeric value. [1..99]</param>
        public void IncrementCounter(double counterNumber)
        {
            Port.Write($"IC {counterNumber}");
        }

        /// <summary>
        /// Fetches data unconditionally from the external input and hand check input.
        /// </summary>
        /// <param name="inputBitNumber">Specify the bit number of input port in integer value. Fetches data of 16 bits
        /// width including the specified bit. [0..32767]</param>
        public void InputDirect(uint inputBitNumber = 0)
        {
            Port.Write($"ID {inputBitNumber}");
        }

        /// <summary>
        /// The specified counter value, the coordinate value of the position number or the data of the specified character
        /// string is received according to the PRN command.
        /// </summary>
        /// <param name="channelNumber">Specify the channel number opened by the OPN command. [0..2]</param>
        /// <param name="value">Specify value of selected content. [1..99]</param>
        /// <param name="content">Specify content.</param>
        public void Input(uint channelNumber = 0, uint value = 1, ContentE content = ContentE.Counter)
        {
            Port.Write($"NP {channelNumber},{value},{(int)content}");
        }

        /// <summary>
        /// Overwrites the current position by adding +/- 360 degrees to the joint position of the R-axis. This is done when you want to use shortcut control of the R-axis, or when you want to use endless control.
        /// </summary>
        /// <param name="number">+1: adds 360 degrees to the current joint position on the R-axis; -1: Substract 360 to the current joint position on the R-axis</param>
        public void JointRollChange(int number)
        {
            Port.Write($"JRC {number}");
        }

        /// <summary>
        /// This compares the value of the internal register with a specified value. If larger, the program will jump. The character
        /// string register and the numbers of characters in a specified character string are compared. If the character string
        /// register is larger, the program will jump.
        /// </summary>
        /// <param name="comparedValue">Specify the value compared with the internal register. [-32768..32767]</param>
        /// <param name="branchingLineNumber">Specify the line number to which the program jumps when the value of the internal register is larger than compared value.</param>
        public void IfLarger(string comparedValue, uint branchingLineNumber)
        {
            Port.Write($"LG {comparedValue},{branchingLineNumber}");
        }


        /// <summary>
        /// Reads the current line number stopping.
        /// </summary>
        public void LineRead()
        {
            Port.Write("LR");
        }


        /// <summary>
        /// Reads the program of the specified line number.
        /// <param name="lineNumber">Specify the line number to be read.0 ≦ line number ≦ 32767 (If omitted, reads the current line number stopping)</param>
        /// </summary>
        public void LineRead(uint lineNumber = 0)
        {
            Port.Write($"LR {lineNumber}");
        }

        /// <summary>
        /// Moves the robot continously through the predefined intermediate points between two specified position numbers.
        /// </summary>
        /// <param name="positionNumberA">Specify the top position number moving continuous. [1...999]</param>
        /// <param name="positionNumberB">Specify the last position number moving continuous. [1...999]</param>
        public void MoveContinuous(uint positionNumberA, uint positionNumberB)
        {
            Port.Write($"MC {positionNumberA},{positionNumberB}");
        }

        /// <summary>
        /// Turns each joint the specified angle from the current position. (Joint interpolation)
        /// </summary>
        /// <param name="waistJointAngle">Specify relative amount of each joint turning from the current position.</param>
        /// <param name="shoulderJointAngle">Specify relative amount of each joint turning from the current position.</param>
        /// <param name="elbowJointAngle">Specify relative amount of each joint turning from the current position.</param>
        /// <param name="pitchJointAngle">Specify relative amount of each joint turning from the current position.</param>
        /// <param name="rollJointAngle">Specify relative amount of each joint turning from the current position.</param>
        public void MoveJoint(double waistJointAngle = 0, double shoulderJointAngle = 0, double elbowJointAngle = 0,
                               double pitchJointAngle = 0, double rollJointAngle = 0)
        {
            Port.Write($"MJ {waistJointAngle},{shoulderJointAngle},{elbowJointAngle},{pitchJointAngle},{rollJointAngle}");
        }

        /// <summary>
        /// Moves the hand tip to the specified position. (Joint interpolation)
        /// </summary>
        /// <param name="positionNumber">Specify the destination position number in integer value. [1..999]</param>
        public void Move(uint positionNumber = 0)
        {
            Port.Write($"MO {positionNumber}");
        }

        /// <summary>
        /// Moves the hand tip to the specified position. (Joint interpolation)
        /// </summary>
        /// <param name="positionNumber">Specify the destination position number in integer value. [1..999]</param>
        /// <param name="grab">Specify hand state.</param>
        public void Move(uint positionNumber = 0, GrabE grab = GrabE.Open)
        {
            Port.Write($"MO {positionNumber},{GrabStateToString(grab)}");
        }

        /// <summary>
        /// Moves the tip of hand to a position whose coordinates (position and angle) have been specified. (Joint interpolation)
        /// </summary>
        /// <param name="xCoordinate">Specify the position in XYZ coordinates (mm) of the robot. (Zero for default)</param>
        /// <param name="yCoordinate">Specify the position in XYZ coordinates (mm) of the robot. (Zero for default)</param>
        /// <param name="zCoordinate">Specify the position in XYZ coordinates (mm) of the robot. (Zero for default)</param>
        /// <param name="aTurnAngle">Specify the turning angle of roll and pitch joints in XYZ coordinates (degree) of the robot. (Zero for default)</param>
        /// <param name="bTurnAngle"></param>
        public void MovePosition(double xCoordinate = 0, double yCoordinate = 0, double zCoordinate = 0, double aTurnAngle = 0, double bTurnAngle = 0)
        {
            Port.Write($"MP {xCoordinate},{yCoordinate},{zCoordinate},{aTurnAngle},{bTurnAngle}");
        }

        /// <summary>
        /// Moves to the specified position with specified interpolation, specified speed, specified timer, and specified input and output signal.
        /// </summary>
        /// <param name="speed">Specify the interpolation speed to the destination position. [0...32767[ (Joint interpolation: %; Linear interpolation: mm/s</param>
        /// <param name="timer">Set timer at the destination position after the movement. [0...255]</param>
        /// <param name="outputOn">Set the output signal that turns ON. [0...& FFFF]; 1: Setting; 0: Not setting</param>
        /// <param name="outputOff">Set the output signal that turns OFF. [0...& FFFF]; 1: Setting; 0: Not setting</param>
        /// <param name="inputOn">Set the input waiting signal that turns ON. [0...& FFFF]; 1: Setting; 0: Not setting</param>
        /// <param name="inputOff">Set the input waiting signal that turns OFF. [0...& FFFF]; 1: Setting; 0: Not setting</param>
        /// <param name="interpolation">Specify the interpolation mode to the destination position. [0: Joint interpolation; 1: Linear interpolation; 2: Circular interpolation]</param>
        /// <param name="xCoordinate">Specify the location (mm) in XYZ coordinates of the robot. (Zero by default)</param>
        /// <param name="yCoordinate">Specify the location (mm) in XYZ coordinates of the robot. (Zero by default)</param>
        /// <param name="zCoordinate">Specify the location (mm) in XYZ coordinates of the robot. (Zero by default)</param>
        /// <param name="aTurningAngle">Specify the turning angle around roll in XYZ coordinates (degree) of the robot. (0 for default)</param>
        /// <param name="bTurningAngle">Specify the turning angle around pitch in XYZ coordinates (degree) of the robot. (0 for default)</param>
        public void MovePlayback(double speed, double timer, double outputOn, double outputOff, double inputOn,
                                 double inputOff, double interpolation, double xCoordinate, double yCoordinate,
                                 double zCoordinate, double aTurningAngle, double bTurningAngle)
        {
            Port.Write($"MPB {speed},{timer},{outputOn},{outputOff},{inputOn},{inputOff},{interpolation},{xCoordinate},{yCoordinate},{zCoordinate},{aTurningAngle},{bTurningAngle}");
        }

        /// <summary>
        /// Moves to the specified position with specified interpolation.
        /// </summary>
        /// <param name="interpolation">Specify the interpolation mode to the destination position. [0: Joint interpolation; 1: Linear interpolation; 2: Circular interpolation]</param>
        /// <param name="xCoordinate">Specify the location (mm) in XYZ coordinates of the robot. (Zero by default)</param>
        /// <param name="yCoordinate">Specify the location (mm) in XYZ coordinates of the robot. (Zero by default)</param>
        /// <param name="zCoordinate">Specify the location (mm) in XYZ coordinates of the robot. (Zero by default)</param>
        /// <param name="aTurningAngle">Specify the turning angle around roll in XYZ coordinates (degree) of the robot. (0 for default)</param>
        /// <param name="bTurningAngle">Specify the turning angle around pitch in XYZ coordinates (degree) of the robot. (0 for default)</param>
        public void MovePlaybackContinuous(InterpolationE interpolation, double xCoordinate, double yCoordinate,
                                           double zCoordinate, double aTurningAngle, double bTurningAngle)
        {
            Port.Write($"MPC {(int)interpolation},{xCoordinate},{yCoordinate},{zCoordinate},{aTurningAngle},{bTurningAngle}");
        }

        /// <summary>
        /// Moves the tip of hand through the predefined intermediate positions in circular interpolation.
        /// </summary>
        /// <param name="positionNumberA">Specify the position on the circle. [1...999]</param>
        /// <param name="positionNumberB">Specify the position on the circle. [1...999]</param>
        /// <param name="positionNumberC">Specify the position on the circle. [1...999]</param>
        public void MoveR(uint positionNumberA, uint positionNumberB, uint positionNumberC)
        {
            Port.Write($"MR {positionNumberA},{positionNumberB},{positionNumberC}");
        }

        /// <summary>
        /// Moves to specified position in circulat interpolation.
        /// </summary>
        /// <param name="positionNumber">Specify the destination position. [1...999]</param>
        public void MoveRA(uint positionNumber)
        {
            Port.Write($"MRA {positionNumber}");
        }

        /// <summary>
        /// Moves to specified position in circulat interpolation.
        /// </summary>
        /// <param name="positionNumber">Specify the destination position. [1...999]</param>
        /// <param name="grab">Specify open or close state of the hand.</param>
        public void MoveRA(uint positionNumber, GrabE grab)
        {
            Port.Write($"MRA {positionNumber},{GrabStateToString(grab)}");
        }

        /// <summary>
        /// Moves the tip of hand to the specified position.
        /// </summary>
        /// <param name="positionNumber">Specify the destination position number in integer value. [1...999]</param>
        public void MoveStraight(uint positionNumber)
        {
            Port.Write($"MS {positionNumber}");
        }

        /// <summary>
        /// Moves the tip of hand to a position away from the specified position by the distance as specified in the tool direction. (joint interpolation)
        /// </summary>
        /// <param name="positionNumber">Specify the destination position number in integer value. [1...999]</param>
        /// <param name="travelDistance">Specify the distance in tool direction from the specified position to the destination point. (0 for default)[-3276,80..3276,70]</param>
        public void MoveTool(uint positionNumber, double travelDistance = 0)
        {
            Port.Write($"MT {positionNumber},{travelDistance}");
        }

        /// <summary>
        /// Moves the tip of hand to a position away from the specified position by the distance as specified in the tool direction. (joint interpolation)
        /// </summary>
        /// <param name="positionNumber">Specify the destination position number in integer value. [1...999]</param>
        /// <param name="travelDistance">Specify the distance in tool direction from the specified position to the destination point. (0 for default)[-3276,80..3276,70]</param>
        /// <param name="grab">Specify open or close state of the hand.</param>
        public void MoveTool(uint positionNumber, double travelDistance = 0, GrabE grab = GrabE.Closed)
        {
            Port.Write($"MT {positionNumber},{travelDistance},{GrabStateToString(grab)}");
        }

        /// <summary>
        /// Moves the tip of hand to a position away from the specified position by the distance as specified in the tool direction. (Linear interpolation)
        /// </summary>
        /// <param name="positionNumber">Specify the destination position number in integer value. [1...999]</param>
        /// <param name="travelDistance">Specify the distance in tool direction from the specified position to the destination point. (Zero by default). [-3276,80...3276,70]</param>
        public void MoveToolStraight(uint positionNumber, double travelDistance = 0)
        {
            Port.Write($"MTS {positionNumber},{travelDistance}");
        }

        /// <summary>
        /// Moves the tip of hand to a position away from the specified position by the distance as specified in the tool direction. (Linear interpolation)
        /// </summary>
        /// <param name="positionNumber">Specify the destination position number in integer value. [1...999]</param>
        /// <param name="travelDistance">Specify the distance in tool direction from the specified position to the destination point. (Zero by default). [-3276,80...3276,70]</param>
        /// <param name="grab">Specify open or close state of the hand.</param>
        public void MoveToolStraight(uint positionNumber, double travelDistance = 0, GrabE grab = GrabE.Closed)
        {
            Port.Write($"MTS {positionNumber},{travelDistance},{GrabStateToString(grab)}");
        }

        /// <summary>
        /// Select the specified program.
        /// </summary>
        /// <param name="programName">Specify the robot program name (less than 8 characters).</param>
        public void Number(string programName)
        {
            Port.Write($"N \"{programName}\"");
        }

        /// <summary>
        /// This compares the value of the internal register with a specified value. If not equal, the program will jump. The character
        /// string register and details of a specified character string are compared. If not equal, the program will jump.
        /// </summary>
        /// <param name="comparedValue">Specify the value that the internal register compares contents with. [-32768...32767]</param>
        /// <param name="branchingLineNumber">Specify the line number to which the program jumps when the comparison result is not equal. [1...9999]</param>
        public void IfNotEqual(string comparedValue, uint branchingLineNumber)
        {
            Port.Write($"NE {comparedValue}, {branchingLineNumber}");
        }

        /// <summary>
        /// Carry out origin setting. (The robot moves to the user-defined origin.)
        /// </summary>
        public void Nest()
        {
            Port.Write("NT");
        }

        /// <summary>
        /// Deletes the specified program and position data.
        /// </summary>
        public void New()
        {
            Port.Write("NW");
        }

        /// <summary>
        /// Specifies the range of a loop in a program executed by the RC command.
        /// </summary>
        public void Next()
        {
            Port.Write("NX");
        }

        /// <summary>
        /// Sets the output state of the specified bit through an external output port.
        /// </summary>
        /// <param name="sign">Set ON or OFF state of the specified bit. [+: bit ON;  -: bit OFF]</param>
        /// <param name="bitNumber">Specify the bit number of external output. [0..32767]</param>
        public void OutputBit(SignE sign, uint bitNumber)
        {
            var strSign = sign == SignE.Plus ? "+" : "-";
            Port.Write($"OB {strSign}, {bitNumber}");
        }

        /// <summary>
        /// Outputs the specified counter value unconditionally through the output port.
        /// </summary>
        /// <param name="counterNumber">Specify the counter number to be output. [1..99]</param>
        /// <param name="outputBit">Specify the reference bit number of output data [0..32767]</param>
        /// <param name="bitWidth">Specify bit width of output data. [1..16]</param>
        public void OutputCounter(uint counterNumber, uint outputBit = 0, uint bitWidth = 16)
        {
            Port.Write($"OC {counterNumber},{outputBit},{bitWidth}");
        }


        /// <summary>
        /// Outputs the specified data unconditionally through the output port.
        /// </summary>
        /// <param name="outputData">Specified output data. [-32768..32767]</param>
        /// <param name="outputBit">Specify the reference bit number of output data [0..32767]</param>
        /// <param name="bitWidth">Specify bit width of output data. [1..16]</param>
        public void OutputDirect(double outputData, uint outputBit = 0, uint bitWidth = 16)
        {
            Port.Write($"OD {outputData},{outputBit},{bitWidth}");
        }

        /// <summary>
        /// Moves to the user-defined origin. (Joint interpolation)
        /// </summary>
        public void Origin()
        {
            Port.Write("OG");
        }

        /// <summary>
        /// Opens communication channel and specify input/output device.
        /// </summary>
        /// <param name="channelNumber">Specify input/output channel number. [0..2]</param>
        /// <param name="deviceNumber">Specify input/ourput device number. [1: standard RS-232C;  2: standard RS-422]</param>
        public void Open(uint channelNumber, uint deviceNumber)
        {
            Port.Write($"OPN {channelNumber}, {deviceNumber}");
        }

        /// <summary>
        /// ORs the specified data and the internal register data.
        /// </summary>
        /// <param name="operationData">Specify the data to be operated. [-32768..32767]</param>
        public void Or(double operationData)
        {
            Port.Write($"OR {operationData}");
        }

        /// <summary>
        /// Specify program override.
        /// </summary>
        /// <param name="specifiedOverride">Specify override value. (%) [1..200]</param>
        public void Override(double specifiedOverride)
        {
            Port.Write($"OVR {specifiedOverride}");
        }

        /// <summary>
        /// Defines the number of grid points in the column and row directions for the specified pallet.
        /// </summary>
        /// <param name="palletNumber">Specify number of pallet using. [1...9]</param>
        /// <param name="numberOfColumnGridPoints">Set grid points of column of pallet. [1...32767]</param>
        /// <param name="numberOfRowGridPoints">Set grid points of row of pallet. [1...32767]</param>
        public void PalletAssign(uint palletNumber, uint numberOfColumnGridPoints, uint numberOfRowGridPoints)
        {
            Port.Write($"PA {palletNumber},{numberOfColumnGridPoints},{numberOfRowGridPoints}");
        }

        /// <summary>
        /// Clears the data of the specified position (s).
        /// </summary>
        /// <param name="positionNumberA">Specify position number deleting. [1...999]</param>
        public void PositionClear(uint positionNumberA)
        {
            Port.Write($"PC {positionNumberA}");
        }

        /// <summary>
        /// Defines the coordinates (location and angle) of the specified position.
        /// </summary>
        /// <param name="positionNumber">Specify position number defining. [1..999]</param>
        /// <param name="xCoordinate">Specify the location (mm) in X coordinate of the robot. (0 for default)</param>
        /// <param name="yCoordinate">Specify the location (mm) in Y coordinate of the robot. (0 for default)</param>
        /// <param name="zCoordinate">Specify the location (mm) in Z coordinate of the robot. (0 for default)</param>
        /// <param name="aTurningAngle">Specify the turning angle around roll axe in XYZ coordinates (degree) of the robot. (0 for default)</param>
        /// <param name="bTurningAngle">Specify the turning angle around pitch axe in XYZ coordinates (degree) of the robot. (0 for default)</param>
        public void PositionDefine(uint positionNumber = 0, double xCoordinate = 0, double yCoordinate = 0, 
                                    double zCoordinate = 0, double aTurningAngle = 0, double bTurningAngle = 0)
        {
            Port.Write($"PD {positionNumber}, {xCoordinate}, {yCoordinate}, {zCoordinate}, {aTurningAngle}, {bTurningAngle}");
        }

        /// <summary>
        /// Replaces position A by position B.
        /// </summary>
        /// <param name="positionNumberA">Specify the position number. (Destination) [1..999]</param>
        /// <param name="positionNumberB">Specify the position number. (Source) [1..999]</param>
        public void PositionLoad(uint positionNumberA, uint positionNumberB)
        {
            Port.Write($"PL {positionNumberA}, {positionNumberB}");
        }

        /// <summary>
        /// Reads contents of parameter specified.
        /// </summary>
        /// <param name="parameterName">Specify parameter name. Only parameter name defined is valid. (Defined order for default)</param>
        public void ParameterRead(string parameterName)
        {
            Port.Write($"PMR {parameterName}");
        }

        /// <summary>
        /// Renews the contents of the specified parameter.
        /// </summary>
        /// <param name="parameterName">Specify the parameter name changing contents.</param>
        /// <param name="parameterContents">Specify the contents that you want to change to.</param>
        public void ParameterWriting(string parameterName, string parameterContents)
        {
            Port.Write($"PMW {parameterName}, {parameterContents}");
        }

        /// <summary>
        /// Reads the coordinates of the specified position and the open/close state of the hand. (Using RS-232C)
        /// </summary>
        /// <param name="positionNumber">Specify the position number that you want to read. [0...999] (If ommited, the current position number is valid.)</param>
        public void PositionRead(uint positionNumber)
        {
            Port.Write($"PR {positionNumber}");
        }

        /// <summary>
        /// Calculates the coordinates of a grid point on the specified pallet and sets the coordinates value to the specified position.
        /// </summary>
        /// <param name="palletNumber">Specify the number of pallet using. [1..9]</param>
        public void Pallet(uint palletNumber)
        {
            Port.Write($"PT {palletNumber}");
        }

        /// <summary>
        /// Waits for in-position of servomotor about all joints till it becomes within the specified value.
        /// </summary>
        /// <param name="positioningPulse">Specify the judgment pulse number of in-position. [1..10000]</param>
        public void PulseWait(double positioningPulse)
        {
            Port.Write($"PW {positioningPulse}");
        }

        /// <summary>
        /// Exchanges the coordinates of the specified position for those of another specified position.
        /// </summary>
        /// <param name="positionNumberA">Specify the position number exchanging. [1..999]</param>
        /// <param name="positionNumberA2">Specify the position number exchanging. [1..999]</param>
        public void PositionExchange(uint positionNumberA, uint positionNumberA2)
        {
            Port.Write($"PW {positionNumberA}, {positionNumberA2}");
        }

        /// <summary>
        /// Reads the program name or the program information.
        /// </summary>
        /// <param name="programName">Specify the robot program name to be read. (Max. 8 characters)</param>
        public void QuestionNumber(string programName)
        {
            Port.Write($"QN {programName}");
        }

        /// <summary>
        /// Repeats the loop specified by the NX command the specified number of times.
        /// </summary>
        /// <param name="numberOfRepeatedCycles">Specify the number of timer repeating. [1..32767]</param>
        public void RepeatCycle(uint numberOfRepeatedCycles)
        {
            Port.Write($"RC {numberOfRepeatedCycles}");
        }

        /// <summary>
        /// Executes the specified part of commands in a program.
        /// </summary>
        public void Run()
        {
            Port.Write("RN");
        }

        /// <summary>
        /// Executes the specified part of commands in a program.
        /// </summary>
        public void Run(string programName)
        {
            Port.Write($"RN ,,{programName}");
        }

        /// <summary>
        /// Executes the specified part of commands in a program.
        /// </summary>
        /// <param name="startLineNumber">Specify the line number beginning. [1..9999]</param>
        /// <param name="endLineNumber">Specify the line number ending. [1..9999]</param>
        /// <param name="programName">Specify the program name. (Max. 8 characters)</param>
        public void Run(uint startLineNumber, uint endLineNumber, string programName)
        {
            Port.Write($"RN {startLineNumber}, {endLineNumber}, {programName}");
        }

        /// <summary>
        /// Resets the program and error condition.  CHECK MANUAL BEFORE USE (page 3-90)
        /// </summary>
        /// <param name="resetNumber">Specify the contents of reset.</param>
        public void Reset(uint resetNumber)
        {
            Port.Write($"RS {resetNumber}");
        }

        /// <summary>
        /// Completes a subroutine and returns to the main program.
        /// </summary>
        /// <param name="lineNumber">Specify the line number to jump. [1..9999]</param>
        public void Return(uint lineNumber)
        {
            Port.Write($"RT {lineNumber}");
        }

        /// <summary>
        /// A specified value is set in the specified counter or character string.
        /// </summary>
        /// <param name="counterNumber">Specify the number of counter setting. [1..99]</param>
        /// <param name="counterSetValue">Specify the value of counter setting. (0 for default) [-32768..32767]</param>
        public void SetCounter(string counterNumber = "1", string counterSetValue = "0")
        {
            Port.Write($"SC {counterNumber}, {counterSetValue}");
        }

        /// <summary>
        /// Defines the moving velocity, first order time constant, acceleration/deceleration time, and continous path setting.
        /// </summary>
        /// <param name="movingSpeed">Set moving speed at linear or circulat interpolation. [0,01...650](mm/s)</param>
        public void SpeedDefine(double movingSpeed)
        {
            Port.Write($"SD {movingSpeed}");
        }

        /// <summary>
        /// Adds each coordinate value of position B to each coordinate value of position A and defines it again as a new position.
        /// </summary>
        /// <param name="positionNumberA">Specify the position number. [1..999]</param>
        /// <param name="positionNumberB">Specify the position number. [1..999]</param>
        public void Shift(uint positionNumberA, uint positionNumberB)
        {
            Port.Write($"SF {positionNumberA}, {positionNumberB}");
        }

        /// <summary>
        /// This compares the value of the internal register with a specified value. If smaller, the program will jump. The character string
        /// register and the numbers of characters in a specified character string are compared. If the string register is smaller, the
        /// program will jump.
        /// </summary>
        /// <param name="comparedValue">Specify the value compared with the internal register. [-32768..32767]</param>
        /// <param name="branchingLineNumber">Specify the line number to which the program jumps. [1..9999]</param>
        public void IfSmaller(string comparedValue, uint branchingLineNumber)
        {
            Port.Write($"SM {comparedValue}, {branchingLineNumber}");
        }

        /// <summary>
        /// Turns on servo
        /// </summary>
        public void ServoOn()
        {
            Port.Write("SVO");
        }

        /// <summary>
        /// Sets the operating speed, acceleration or deceleration time and the continuous path setting.
        /// </summary>
        /// <param name="speedLevel">Set moving speed. [0...30]</param>
        public void Speed(uint speedLevel)
        {
            Port.Write($"SP {speedLevel}");
        }

        /// <summary>
        /// Reads the contents of the specified step number, or the stopping step number.
        /// </summary>
        /// <param name="stepNumber">Specify the step number reading. [0..999]</param>
        public async Task<string> StepRead(uint stepNumber)
        {
            Port.Write($"STR {stepNumber}");
            await Port.WaitForMessageAsync();
            return Port.Read() ?? string.Empty;
        }

        /// <summary>
        /// Causes a jump to occur in accordance with the specified bit value in the internal register.
        /// </summary>
        /// <param name="sign">Set the condition that compares bit. [+: the bit is ON;  -: the bit is OFF]</param>
        /// <param name="bitNumber">Specify the bit number of the internal register. [0..15]</param>
        /// <param name="branchingLineNumber">Specify the line number to which the program jump. [1..9999]</param>
        public void TestBit(SignE sign, uint bitNumber, uint branchingLineNumber)
        {
            var strSing = sign == SignE.Plus ? "+" : "-";
            Port.Write($"TB {strSing}, {bitNumber}, {branchingLineNumber}");
        }

        /// <summary>
        /// Causes a jump to occur in accordance with the specified bit value in the external input.
        /// </summary>
        /// <param name="sign">Set the condition that compares bit. [+: the bit is ON;  -: the bit is OFF]</param>
        /// <param name="inputBitNumber">Specify the bit number of general input. [0..32767]</param>
        /// <param name="branchingLineNumber">Specify the line number to which the program jumps. [1..9999]</param>
        public void TestBitDirect(SignE sign, uint inputBitNumber, uint branchingLineNumber)
        {
            var strSing = sign == SignE.Plus ? "+" : "-";
            Port.Write($"TB {strSing}, {inputBitNumber}, {branchingLineNumber}");
        }

        /// <summary>
        /// Halts the motion for the specified length of time.
        /// </summary>
        /// <param name="timerCounter">Set the period of timer. [0..32767]</param>
        public void Timer(double timerCounter)
        {
            Port.Write($"TI {timerCounter}");
        }

        /// <summary>
        /// Establishes the distance between the hand mounting surface and the tip of hand.
        /// </summary>
        /// <param name="toolLength">Set the distance from the hand mounting surface to the tip of hand. [0...300](mm)</param>
        public void Tool(double toolLength)
        {
            Port.Write($"TL {toolLength}");
        }

        /// <summary>
        /// Reads the software version of the system ROM.
        /// </summary>
        public void VersionRead()
        {
            Port.Write("VR");
        }

        /// <summary>
        /// Reads the coordinates of the current position and the open or close state of the hand. (Using RS-232C)
        /// </summary>
        public async Task<Position> Where()
        {
            Port.Write("WH");
            await Port.WaitForMessageAsync();
            return Position.FromWHResponse(Port.Read());
        }

        /// <summary>
        /// Reads the tool length currently being established. (Using RS-232C)
        /// </summary>
        public void WhatTool()
        {
            Port.Write("WT");
        }

        /// <summary>
        /// EXCLUSIVE ORs the specified data and the internal register data.
        /// </summary>
        /// <param name="operationData">Specify the data to be operated. [-32768..32767]</param>
        public void ExclusiveOr(uint operationData)
        {
            Port.Write($"XO {operationData}");
        }

        /// <summary>
        /// Allows the programmer to write a comment. [up to 120 characters]
        /// </summary>
        /// <param name="comment"></param>
        public void Comment(string comment)
        {
            Port.Write($"' {comment}");
        }

        private static string GrabStateToString(GrabE grab)
        {
            return grab == GrabE.Open ? "O" : "C";
        }

        // TODO: Delete later - it's only for debugging purposes
        private void Port_DataReceived(string data)
        {
            Debug.WriteLine(data);
        }
    }
}
