using MindWeaveClient.ViewModel.Main;
using System.Windows;

namespace MindWeaveClient.View.Settings
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            this.DataContext = new SettingsViewModel(this);
        }

    }
}