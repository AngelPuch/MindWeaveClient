using MindWeaveClient.ViewModel.Authentication;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;

namespace MindWeaveClient.View.Authentication
{
    public partial class VerificationPage : Page
    {
        public VerificationPage(VerificationViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }
    }
}
