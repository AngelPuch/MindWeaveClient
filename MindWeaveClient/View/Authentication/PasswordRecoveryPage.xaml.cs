// MindWeaveClient/View/Authentication/PasswordRecoveryPage.xaml.cs
using MindWeaveClient.ViewModel.Authentication;
using System;
using System.Linq;
using System.Text.RegularExpressions; // Necesario para Regex
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MindWeaveClient.View.Authentication
{
    public partial class PasswordRecoveryPage : Page
    {
        public PasswordRecoveryPage()
        {
            //InitializeComponent();
            //this.DataContext = new PasswordRecoveryViewModel(
                //() => { if (this.NavigationService.CanGoBack) this.NavigationService.GoBack(); },
                //() => this.NavigationService?.Navigate(new LoginPage())
            //);
        }

        // Valida que solo se ingresen números en el TextBox del código
        private void CodeTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+"); // Solo permite números
            e.Handled = regex.IsMatch(e.Text);
        }

        // --- ELIMINADOS ---
        // private void CodeTextBox_TextChanged(object sender, TextChangedEventArgs e) { ... }
        // private void CodeTextBox_PreviewKeyDown(object sender, KeyEventArgs e) { ... }
    }
}