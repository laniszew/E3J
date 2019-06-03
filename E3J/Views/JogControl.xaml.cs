using System.Windows.Controls;
using E3J.ViewModels;
using PropertyChanged;

namespace E3J.Views
{
    /// <summary>
    /// Interaction logic for JogControl.xaml
    /// </summary>
    [DoNotNotify]
    public partial class JogControl : Page
    {
        public JogControl()
        {
            InitializeComponent();
            DataContext = new JogControlViewModel();
        }
    }
}