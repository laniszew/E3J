using System.Windows;
using System.Windows.Controls;
using E3J.ViewModels;

namespace E3J.Views
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : Page
    {
        public MainView()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
        }
    }
}
