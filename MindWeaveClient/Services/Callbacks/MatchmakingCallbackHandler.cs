using MindWeaveClient.MatchmakingService;
using MindWeaveClient.Services.Abstractions;
using MindWeaveClient.Utilities.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace MindWeaveClient.Services.Callbacks
{
    [CallbackBehavior(UseSynchronizationContext = false)]
    public class MatchmakingCallbackHandler : IMatchmakingManagerCallback
    {
        private readonly ICurrentMatchService _currentMatchService;
        private readonly IDialogService _dialogService;
  
        public event Action<LobbyStateDto> OnLobbyStateUpdatedEvent;
        public event Action<string, List<string>, LobbySettingsDto, string> OnMatchFoundEvent;
        public event Action<string> OnLobbyCreationFailedEvent;
        public event Action<string> OnKickedEvent;

        public MatchmakingCallbackHandler(
            ICurrentMatchService currentMatchService,
            IDialogService dialogService)
        {
            _currentMatchService = currentMatchService;
            _dialogService = dialogService;
        }


        public void kickedFromLobby(string reason)
        {
            OnKickedEvent?.Invoke(reason);

            App.Current.Dispatcher.Invoke(() =>
            {
                _dialogService.showInfo(reason, Properties.Langs.Lang.KickedMessage);
            });
        }

        public void lobbyCreationFailed(string reason)
        {
            OnLobbyCreationFailedEvent?.Invoke(reason);

            App.Current.Dispatcher.Invoke(() =>
            {
                _dialogService.showError(reason, Properties.Langs.Lang.ErrorTitle);
            });
        }

        public void matchFound(string lobbyCode, string[] players, LobbySettingsDto settings, string puzzleImagePath)
        {
            List<string> playerList = players?.ToList() ?? new List<string>();

            if (_currentMatchService != null)
            {
                _currentMatchService.initializeMatch(lobbyCode, playerList, settings, puzzleImagePath);
            }

            OnMatchFoundEvent?.Invoke(lobbyCode, playerList, settings, puzzleImagePath);
        }

        public void onGameStarted(MindWeaveClient.MatchmakingService.PuzzleDefinitionDto puzzleDefinition)
        {
            if (puzzleDefinition == null || _currentMatchService == null)
            {
                return;
            }

            // 1. Creamos el nuevo DTO del tipo 'PuzzleManagerService'
            var puzzleManagerDto = new MindWeaveClient.PuzzleManagerService.PuzzleDefinitionDto
            {
                fullImageBytes = puzzleDefinition.fullImageBytes,
                puzzleHeight = puzzleDefinition.puzzleHeight,
                puzzleWidth = puzzleDefinition.puzzleWidth,

                // 2. Mapeamos la lista de piezas una por una
                pieces = puzzleDefinition.pieces.Select(piece =>
                    new MindWeaveClient.PuzzleManagerService.PuzzlePieceDefinitionDto
                    {
                        pieceId = piece.pieceId,
                        sourceX = piece.sourceX,
                        sourceY = piece.sourceY,
                        width = piece.width,
                        height = piece.height,
                        correctX = piece.correctX,
                        correctY = piece.correctY
                    }).ToArray() // Convertimos el resultado a un array
            };

            _currentMatchService?.setPuzzle(puzzleManagerDto);
        }

        public void updateLobbyState(LobbyStateDto lobbyStateDto)
        {
  
            OnLobbyStateUpdatedEvent?.Invoke(lobbyStateDto);
        }
    }
}