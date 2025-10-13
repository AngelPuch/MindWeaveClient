using MindWeaveClient.ViewModel.Main;
using System.Windows;
using System.Windows.Controls;

namespace MindWeaveClient.View.Main
{
    public partial class SelectAvatarPage : Page
    {
        public SelectAvatarPage()
        {
            InitializeComponent();
        }

        // Este método se llama cuando un avatar es seleccionado en la galería
        // y actualiza la propiedad 'selectedAvatar' en el ViewModel.
        private void OnAvatarSelected(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is Avatar avatar)
            {
                if (element.Tag is SelectAvatarViewModel viewModel)
                {
                    viewModel.selectedAvatar = avatar;
                }
            }
        }
    }
}