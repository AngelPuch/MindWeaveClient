using MindWeaveClient.ViewModel.Main;
using System.Windows.Controls;

namespace MindWeaveClient.View.Main
{
    public partial class SelectionPuzzlePage : Page
    {
        public SelectionPuzzlePage(SelectionPuzzleViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }
    }
}
