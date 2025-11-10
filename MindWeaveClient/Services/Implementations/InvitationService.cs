using MindWeaveClient.Properties.Langs;
using MindWeaveClient.Services.Abstractions;
using MindWeaveClient.Utilities.Abstractions;
using MindWeaveClient.View.Game;
using System;
using System.Linq;
using System.Windows;

namespace MindWeaveClient.Services.Implementations
{
    public class InvitationService : IInvitationService
    {
        private readonly IDialogService dialogService;
        private readonly IMatchmakingService matchmakingService;
        private readonly IWindowNavigationService windowNavigationService;
        private readonly ICurrentLobbyService currentLobbyService;
        private readonly ISocialService socialService;

        private bool isProcessingInvite = false;

        public InvitationService(
            IDialogService dialogService,
            IMatchmakingService matchmakingService,
            IWindowNavigationService windowNavigationService,
            ICurrentLobbyService currentLobbyService,
            ISocialService socialService)
        {
            this.dialogService = dialogService;
            this.matchmakingService = matchmakingService;
            this.windowNavigationService = windowNavigationService;
            this.currentLobbyService = currentLobbyService;
            this.socialService = socialService;
        }

        public void subscribeToGlobalInvites()
        {
            socialService.LobbyInviteReceived += onLobbyInviteReceived;
        }

        public void unsubscribeFromGlobalInvites()
        {
            socialService.LobbyInviteReceived -= onLobbyInviteReceived;
        }

        private async void onLobbyInviteReceived(string fromUsername, string lobbyId)
        {
            // Prevenir procesamiento duplicado
            if (isProcessingInvite)
            {
                System.Diagnostics.Trace.TraceWarning("Already processing an invite, ignoring duplicate.");
                return;
            }

            // Ignorar auto-invitaciones
            if (fromUsername.Equals(SessionService.Username, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // Verificar si ya está en un lobby o juego
            var gameWindow = Application.Current.Windows.OfType<GameWindow>().FirstOrDefault();
            if (gameWindow != null)
            {
                dialogService.showWarning(
                    "Cannot accept invite while in a game.",
                    Lang.WarningTitle);
                return;
            }

            isProcessingInvite = true;

            try
            {
                string message = string.Format(
                    Lang.InviteReceivedBody ?? "You received an invitation from {0} to join lobby {1}. Do you want to join?",
                    fromUsername,
                    lobbyId);

                bool result = dialogService.showConfirmation(message, Lang.InviteReceivedTitle ?? "Lobby Invitation");

                if (result == true)
                {
                    await joinLobbyFromInvite(lobbyId);
                }
            }
            finally
            {
                isProcessingInvite = false;
            }
        }

        private async System.Threading.Tasks.Task joinLobbyFromInvite(string lobbyId)
        {
            try
            {
                if (string.IsNullOrEmpty(SessionService.Username))
                {
                    dialogService.showError(Lang.ErrorTitle, Lang.ErrorSessionExpired ?? "Session expired");
                    return;
                }

                // Unirse al lobby
                await matchmakingService.joinLobbyAsync(SessionService.Username, lobbyId);

                // Limpiar el estado inicial del lobby (se recibirá por callback)
                currentLobbyService.setInitialState(null);

                // Navegar a la ventana de juego
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var mainWindow = Application.Current.Windows
                        .OfType<View.Main.MainWindow>()
                        .FirstOrDefault();

                    if (mainWindow != null)
                    {
                        windowNavigationService.closeWindowFromContext(mainWindow);
                    }

                    windowNavigationService.openWindow<GameWindow>();
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError($"Error joining lobby from invite: {ex.Message}");
                dialogService.showError(
                    Lang.ErrorTitle,
                    Lang.ErrorJoiningLobby ?? $"Failed to join lobby: {ex.Message}");

                matchmakingService.disconnect();
            }
        }
    }
}