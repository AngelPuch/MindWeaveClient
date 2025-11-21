using MindWeaveClient.ViewModel.Main;
using System;
using System.Windows;
using System.Windows.Controls;

namespace MindWeaveClient.View.Main
{
    public partial class MainMenuPage : Page
    {
        public MainMenuPage(MainMenuViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
            this.Unloaded += mainMenuPageUnloaded;
        }

        private void mainMenuPageUnloaded(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is IDisposable disposableViewModel)
            {
                disposableViewModel.Dispose();
            }
        }
    }
}
