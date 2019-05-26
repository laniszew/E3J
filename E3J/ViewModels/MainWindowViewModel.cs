using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using Driver;

namespace E3J.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private DriverSettings Settings { get; set; } 

        private E3JManipulator Manipulator { get; set; }
       

        public MainWindowViewModel()
        {
            Settings = DriverSettings.CreateDefaultSettings();
            Manipulator = new E3JManipulator(Settings);
        }
      
    }
}
