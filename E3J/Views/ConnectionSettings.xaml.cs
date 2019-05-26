using System.Windows.Controls;
using E3J.ViewModels;

namespace E3J.Views
{
    /// <summary>
    /// Interaction logic for ConnectionSettings.xaml
    /// </summary>
    public partial class ConnectionSettings
    {
        public ConnectionSettings()
        {
            InitializeComponent();
            DataContext = new ConnectionSettingsViewModel();
        }
    }
}
