using System.Windows;
using MindWeaveClient.ViewModel.Main;

namespace MindWeaveClient.View.Main
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow(SettingsViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;

            viewModel.setCloseAction(
                (result) => { this.DialogResult = result; },
                this.Close
            );
        }

    }
}