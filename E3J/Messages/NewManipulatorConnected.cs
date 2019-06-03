using Driver;

namespace E3J.Messages
{
    public class NewManipulatorConnected
    {
        public E3JManipulator Manipulator { get; }

        public NewManipulatorConnected(E3JManipulator manipulator)
        {
            Manipulator = manipulator;
        }
    }
}