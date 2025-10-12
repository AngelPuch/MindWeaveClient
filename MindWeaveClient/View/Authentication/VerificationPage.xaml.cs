using MindWeaveClient.ViewModel.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MindWeaveClient.View.Authentication
{
    /// <summary>
    /// Lógica de interacción para VerificationPage.xaml
    /// </summary>
    public partial class VerificationPage : Page
    {
        public VerificationPage(string email)
        {
            InitializeComponent();
            this.DataContext = new VerificationViewModel(
                email,
                page => this.NavigationService?.Navigate(page),
                () => { if (this.NavigationService.CanGoBack) this.NavigationService.GoBack(); }
            );
        }

        private void CodeTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+"); // Solo permite números
            e.Handled = regex.IsMatch(e.Text);
        }


    }
}
