using MindWeaveClient.MatchmakingService;
using MindWeaveClient.Services.Abstractions;
using MindWeaveClient.Utilities.Abstractions;
using MindWeaveClient.View.Game; // <-- AÑADIR
using System;
using System.Linq;

namespace MindWeaveClient.Services.Callbacks
{
    public class MatchmakingCallbackHandler : IMatchmakingServiceCallback
    {
        private readonly ICurrentLobbyService currentLobbyService;
        private readonly INavigationService navigationService;
        private readonly IDialogService dialogService;
        private readonly ICurrentMatchService currentMatchService; // <-- AÑADIR ESTE CAMPO

        // --- CONSTRUCTOR ACTUALIZADO ---
        public MatchmakingCallbackHandler(
            ICurrentLobbyService currentLobbyService,
            INavigationService navigationService,
            IDialogService dialogService,
            ICurrentMatchService currentMatchService) // <-- AÑADIR ESTE PARÁMETRO
        {
            this.currentLobbyService = currentLobbyService;
            this.navigationService = navigationService;
            this.dialogService = dialogService;
            this.currentMatchService = currentMatchService; // <-- AÑADIR ESTA LÍNEA
        }

        public void kickedFromLobby(string reason)
        {
            currentLobbyService.clearLobby();
            navigationService.goBack();
            dialogService.showInfo(reason, "Kicked");
        }

        public void lobbyCreationFailed(string reason)
        {
            dialogService.showError(reason, "Error");
        }

        // --- ¡MÉTODO MODIFICADO! ---
        // 'matchFound' ahora es 'onGameStarted' y recibe el puzzle
        public void onGameStarted(PuzzleDefinitionDto puzzleDefinition)
        {
            // 1. Guardar el puzzle en el servicio
            currentMatchService.setPuzzle(puzzleDefinition);

            // 2. Navegar a la página del juego
            navigationService.navigate(typeof(GamePage));
        }

        public void updateLobbyState(LobbyStateDto lobbyStateDto)
        {
            currentLobbyService.updateLobbyState(lobbyStateDto);
            if (navigationService.getCurrentPageType() != typeof(LobbyPage))
            {
                navigationService.navigate(typeof(LobbyPage));
            }
        }

        // --- MÉTODO ANTIGUO (probablemente deba borrarse si actualizaste la ref. del servicio) ---
        public void matchFound(string matchId, string[] players)
        {
            // Esta lógica ahora está en onGameStarted
            // Mantenla si tu servidor AÚN no está actualizado
        }
    }
}