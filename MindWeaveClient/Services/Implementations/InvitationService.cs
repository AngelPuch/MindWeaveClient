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

        private bool isProcessingInvite;

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
            if (isProcessingInvite)
            {
                return;
            }

            if (fromUsername.Equals(SessionService.Username, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var gameWindow = Application.Current.Windows.OfType<GameWindow>().FirstOrDefault();
            if (gameWindow != null)
            {
                dialogService.showWarning(
                    Lang.CannotAcceptInviteAlreadyInGame,
                    Lang.WarningTitle);
                return;
            }

            isProcessingInvite = true;

            try
            {
                string message = string.Format(
                    Lang.InviteReceivedBody,
                    fromUsername,
                    lobbyId);

                bool result = dialogService.showConfirmation(message, Lang.InviteReceivedTitle);

                if (result)
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
                    dialogService.showError(Lang.ErrorTitle, Lang.ErrorSessionExpired);
                    return;
                }

                await matchmakingService.joinLobbyAsync(SessionService.Username, lobbyId);
                currentLobbyService.setInitialState(null);

                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    windowNavigationService.openWindow<GameWindow>();
                    windowNavigationService.closeWindow<View.Main.MainWindow>();
                });
            }
            catch (Exception ex)
            {
                dialogService.showError(Lang.ErrorJoiningLobby + ex.Message, Lang.ErrorTitle);
                matchmakingService.disconnect();
            }
        }
    }
}