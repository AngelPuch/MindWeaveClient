using System.Windows.Controls;
using MindWeaveClient.ViewModel.Game;

namespace MindWeaveClient.View.Game
{
    public partial class PostMatchResultsPage : Page
    {
        public PostMatchResultsPage(PostMatchResultsViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }
    }
}
