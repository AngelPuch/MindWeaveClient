using MindWeaveClient.ViewModel.Main;
using System.Windows.Controls;

namespace MindWeaveClient.View.Main
{
    public partial class SelectAvatarPage : Page
    {
        public SelectAvatarPage(SelectAvatarViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }
    }
}