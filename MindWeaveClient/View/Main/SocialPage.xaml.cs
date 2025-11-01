using MindWeaveClient.ViewModel.Main; 
using System.Windows.Controls;
using System.Windows.Input; 

namespace MindWeaveClient.View.Main
{
    public partial class SocialPage : Page
    {
        private SocialViewModel _viewModel;

        public SocialPage()
        {
            InitializeComponent();
            _viewModel = new SocialViewModel(() => NavigationService?.GoBack());
            DataContext = _viewModel;

            Unloaded += SocialPage_Unloaded;
        }

        private void SocialPage_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            _viewModel?.cleanup();
            Unloaded -= SocialPage_Unloaded;
        }

        private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && _viewModel.SearchCommand.CanExecute(null))
            {
                _viewModel.SearchCommand.Execute(null);
            }
        }
    }
}