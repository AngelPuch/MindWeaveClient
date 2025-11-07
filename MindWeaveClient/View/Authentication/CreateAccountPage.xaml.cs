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
        public CreateAccountPage(CreateAccountViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }
    }
}