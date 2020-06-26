using System.Windows.Controls;
using IDE.Common.ViewModels;

namespace E3J.Views
{
    /// <summary>
    /// Interaction logic for Editor.xaml
    /// </summary>
    public partial class Editor : Page
    {
        public Editor()
        {
            InitializeComponent();
            DataContext = new EditorViewModel(CommandHistory, CommandInput);
        }
    }
}
