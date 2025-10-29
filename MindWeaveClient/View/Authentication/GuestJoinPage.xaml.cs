// MindWeaveClient/View/Authentication/GuestJoinPage.xaml.cs
using MindWeaveClient.ViewModel.Authentication;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;

namespace MindWeaveClient.View.Authentication
{
    public partial class GuestJoinPage : Page
    {
        public GuestJoinPage()
        {
            InitializeComponent();
            this.DataContext = new GuestJoinViewModel(
                page => this.NavigationService?.Navigate(page), // Navegar adelante
                () => { if (this.NavigationService.CanGoBack) this.NavigationService.GoBack(); } // Navegar atrás
            );
        }

        // Solo permite caracteres alfanuméricos para el código de lobby
        private void CodeTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Permite solo letras (A-Z) y números (0-9)
            Regex regex = new Regex("^[a-zA-Z0-9]+$");
            e.Handled = !regex.IsMatch(e.Text);
        }
    }
}