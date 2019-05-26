using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using Driver;

namespace E3J.ViewModels
{
    public class JogControlViewModel : ViewModelBase
    {
        private E3JManipulator manipulator;

        public E3JManipulator Manipulator
        {
            get
            {
                return manipulator;
            }
            private set
            {
                manipulator = value;
                RaisePropertyChanged("Manipulator");
            }
        }
        public JogControlViewModel()
        {
           
        }

        [Command]
        public void OpenCloseHandCommand()
        {
            manipulator.GrabOpen();
        }
    }
}
