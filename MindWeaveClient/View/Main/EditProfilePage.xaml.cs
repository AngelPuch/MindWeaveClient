using MindWeaveClient.ViewModel.Main;
using System.Windows;
using System.Windows.Controls;

namespace MindWeaveClient.View.Main
{
    public partial class EditProfilePage : Page
    {
        public EditProfilePage(EditProfileViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
            this.Loaded += EditProfilePage_Loaded;
        }

        private void EditProfilePage_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is EditProfileViewModel vm)
            {
                vm.refreshAvatar();
            }
        }
    }
}
