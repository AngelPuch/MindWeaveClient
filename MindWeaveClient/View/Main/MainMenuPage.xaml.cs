using MindWeaveClient.ViewModel.Main;
using System.Windows.Controls;

namespace MindWeaveClient.View.Main
{
    public partial class MainMenuPage : Page
    {
        public MainMenuPage(MainMenuViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }
    }
}
