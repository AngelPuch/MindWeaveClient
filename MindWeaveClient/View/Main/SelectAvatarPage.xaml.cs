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
        // y actualiza la propiedad 'SelectedAvatar' en el ViewModel.
        private void OnAvatarSelected(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is Avatar avatar)
            {
                // Buscamos el ViewModel en el DataContext del ItemsControl mediante Tag
                if (element.Tag is SelectAvatarViewModel viewModel)
                {
                    viewModel.SelectedAvatar = avatar;
                }
                else
                {
                    // Alternativa: intentar obtener el DataContext de la página
                    if (this.DataContext is SelectAvatarViewModel vm)
                    {
                        vm.SelectedAvatar = avatar;
                    }
                }
            }
        }
    }
}