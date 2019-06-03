using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using Driver;
using E3J.Messages;

namespace E3J.ViewModels
{
    public class JogControlViewModel : ViewModelBase
    {
        public int JogSpeed { get; set; }
        public int JogIncrement { get; set; }

        public E3JManipulator Manipulator { get; set; }

        public JogControlViewModel()
        {
            Messenger.Default.Register<NewManipulatorConnected>(this, message => Manipulator = message.Manipulator);
        }

        public bool CanExecuteOpenCloseHandCommand()
            => Manipulator != null;

        [Command(CanExecuteMethodName = "CanExecuteOpenCloseHandCommand")]
        public async void OpenCloseHandCommand()
        {
            var position = await Manipulator.Where();
            if (position.Grab == E3JManipulator.GrabE.Closed)
            {
                Manipulator.GrabOpen();
            }
            else
            {
                Manipulator.GrabClose();
            }
        }

        [Command]
        public void WaistLeftCommand()
        {
            Manipulator.MoveJoint(-JogIncrement);
        }

        [Command]
        public void WaistRightCommand()
        {
            Manipulator.MoveJoint(JogIncrement);
        }

        [Command]
        public void ShoulderLeftCommand()
        {
            Manipulator.MoveJoint(0, -JogIncrement);
        }

        [Command]
        public void ShoulderRightCommand()
        {
            Manipulator.MoveJoint(0, JogIncrement);
        }

        [Command]
        public void ElbowLeftCommand()
        {
            Manipulator.MoveJoint(0, 0, -JogIncrement);
        }

        [Command]
        public void ElbowRightCommand()
        {
            Manipulator.MoveJoint(0, 0, JogIncrement);
        }

        [Command]
        public void PitchLeftCommand()
        {
            Manipulator.MoveJoint(0, 0, 0, -JogIncrement);
        }

        [Command]
        public void PitchRightCommand()
        {
            Manipulator.MoveJoint(0, 0, 0, JogIncrement);
        }

        [Command]
        public void RollLeftCommand()
        {
            Manipulator.MoveJoint(0, 0, 0, 0, -JogIncrement);
        }

        [Command]
        public void RollRightCommand()
        {
            Manipulator.MoveJoint(0, 0, 0, 0, JogIncrement);
        }

        [Command]
        public void JogSpeedCommand()
        {
            Manipulator.Speed((uint) JogSpeed);
        }
    }
}
