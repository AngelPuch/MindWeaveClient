using MindWeaveClient.ViewModel.Main; // Namespace del ViewModel
using System.Windows.Controls;
using System.Windows.Input; // Para KeyEventArgs

namespace MindWeaveClient.View.Main
{
    public partial class SocialPage : Page
    {
        private SocialViewModel _viewModel;

        public SocialPage()
        {
            InitializeComponent();
            // Crear e asignar el ViewModel. Pasar la acción de navegación hacia atrás.
            _viewModel = new SocialViewModel(() => NavigationService?.GoBack());
            DataContext = _viewModel;

            // Suscribirse al evento Unloaded para limpiar recursos
            Unloaded += SocialPage_Unloaded;
        }

        private void SocialPage_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            // Limpiar suscripciones de callback cuando la página se descarga
            _viewModel?.Cleanup();
            // Eliminar suscripción al evento Unloaded para evitar fugas de memoria
            Unloaded -= SocialPage_Unloaded;
        }

        // Permite buscar presionando Enter en el TextBox de búsqueda
        private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && _viewModel.SearchCommand.CanExecute(null))
            {
                _viewModel.SearchCommand.Execute(null);
            }
        }
    }
}