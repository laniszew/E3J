using System.Windows.Controls;
using E3J.ViewModels;
using PropertyChanged;

namespace E3J.Views
{
    /// <summary>
    /// Interaction logic for ConnectionSettings.xaml
    /// </summary>
    [DoNotNotify]
    public partial class ConnectionSettings
    {
        public ConnectionSettings()
        {
            InitializeComponent();
            DataContext = new ConnectionSettingsViewModel();
        }
    }
}
