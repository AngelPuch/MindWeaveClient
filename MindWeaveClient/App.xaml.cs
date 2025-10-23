using System.Threading;
using System.Windows;
using MindWeaveClient.Services;

namespace MindWeaveClient
{
    /// <summary>
    /// Lógica de interacción para App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var langCode = MindWeaveClient.Properties.Settings.Default.languageCode;
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(langCode);
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            SocialServiceClientManager.Instance.Disconnect();
            MatchmakingServiceClientManager.Instance.Disconnect(); // También el de matchmaking si lo usas
            // Añadir otros servicios si es necesario

            base.OnExit(e);
        }
    }
}
