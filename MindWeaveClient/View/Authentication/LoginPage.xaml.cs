using MindWeaveClient.ViewModel.Authentication;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace MindWeaveClient.View.Authentication
{
    public partial class LoginPage : Page
    {
        public LoginPage(LoginViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }

        private void ToggleShowPass_OnClick(object sender, RoutedEventArgs e)
        {
            bool isVisible = ToggleShowPass.IsChecked == true;

            if (isVisible)
            {
                txtVisiblePassword.Focus();
                txtVisiblePassword.CaretIndex = txtVisiblePassword.Text.Length;
            }
            else
            {
                pbPassword.Focus();
                try
                {
                    var selectMethod = pbPassword.GetType().GetMethod("Select",
                        BindingFlags.Instance | BindingFlags.NonPublic);

                    selectMethod?.Invoke(pbPassword, new object[] { pbPassword.Password.Length, 0 });
                }
                catch
                {
                    // Si falla por permisos de seguridad restringidos en el SO, simplemente se queda al inicio.
                    // Pero en Windows Desktop normal funcionará perfecto.
                }
            }
        }
    }
}