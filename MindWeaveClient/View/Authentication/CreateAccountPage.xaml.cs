using MindWeaveClient.ViewModel.Authentication;
using System;
using System.Windows.Controls;

namespace MindWeaveClient.View.Authentication
{
    /// <summary>
    /// Lógica de interacción para CreateAccountPage.xaml
    /// </summary>
    public partial class CreateAccountPage : Page
    {
        public CreateAccountPage(Action<Page> navigateAction)
        {
            InitializeComponent();
            DataContext = new CreateAccountViewModel(navigateAction);
        }
    }
}