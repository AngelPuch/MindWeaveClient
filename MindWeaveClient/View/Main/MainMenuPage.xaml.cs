using MindWeaveClient.ViewModel.Main;
using System;
using System.Windows.Controls;

namespace MindWeaveClient.View.Main
{
    /// <summary>
    /// Lógica de interacción para MainMenuPage.xaml
    /// </summary>
    public partial class MainMenuPage : Page
    {
        public MainMenuPage(Action<Page> navigateTo)
        {
            InitializeComponent();
            DataContext = new MainMenuViewModel(navigateTo, this);
        }
    }
}
