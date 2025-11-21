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
        private readonly ICurrentMatchService currentMatchService;
        private readonly IDialogService dialogService;
  
        public event Action<LobbyStateDto> OnLobbyStateUpdatedEvent;
        public event Action<string, List<string>, LobbySettingsDto, string> OnMatchFoundEvent;
        public event Action<string> OnLobbyCreationFailedEvent;
        public event Action<string> OnKickedEvent;

        public static event Action<int, string> PieceDragStartedHandler;
        public static event Action<int, double, double, string, int> PiecePlacedHandler;
        public static event Action<int, double, double, string> PieceMovedHandler;
        public static event Action<int, string> PieceDragReleasedHandler;
        public static event Action OnGameStartedNavigation;


        public MatchmakingCallbackHandler(
            ICurrentMatchService currentMatchService,
            IDialogService dialogService)
        {
            this.currentMatchService = currentMatchService;
            this.dialogService = dialogService;
        }


        public void kickedFromLobby(string reason)
        {
            OnKickedEvent?.Invoke(reason);

            App.Current.Dispatcher.Invoke(() =>
            {
                dialogService.showInfo(reason, Properties.Langs.Lang.KickedMessage);
            });
        }

        public void lobbyCreationFailed(string reason)
        {
            OnLobbyCreationFailedEvent?.Invoke(reason);

            App.Current.Dispatcher.Invoke(() =>
            {
                dialogService.showError(reason, Properties.Langs.Lang.ErrorTitle);
            });
        }

        public void matchFound(string lobbyCode, string[] players, LobbySettingsDto settings, string puzzleImagePath)
        {
            List<string> playerList = players?.ToList() ?? new List<string>();

            if (currentMatchService != null)
            {
                currentMatchService.initializeMatch(lobbyCode, playerList, settings, puzzleImagePath);
            }

            OnMatchFoundEvent?.Invoke(lobbyCode, playerList, settings, puzzleImagePath);
        }

        public void onGameStarted(PuzzleDefinitionDto puzzleDefinition)
        {
            if (puzzleDefinition == null || currentMatchService == null)
            {
                return;
            }

            var puzzleManagerDto = new MindWeaveClient.PuzzleManagerService.PuzzleDefinitionDto
            {
                FullImageBytes = puzzleDefinition.FullImageBytes,
                PuzzleHeight = puzzleDefinition.PuzzleHeight,
                PuzzleWidth = puzzleDefinition.PuzzleWidth,

                Pieces = puzzleDefinition.Pieces.Select(piece =>
                    new MindWeaveClient.PuzzleManagerService.PuzzlePieceDefinitionDto
                    {
                        PieceId = piece.PieceId,
                        SourceX = piece.SourceX,
                        SourceY = piece.SourceY,
                        Width = piece.Width,
                        Height = piece.Height,
                        CorrectX = piece.CorrectX,
                        CorrectY = piece.CorrectY,
                        InitialX = piece.InitialX,
                        InitialY = piece.InitialY,
                        TopNeighborId = piece.TopNeighborId,
                        BottomNeighborId = piece.BottomNeighborId,
                        LeftNeighborId = piece.LeftNeighborId,
                        RightNeighborId = piece.RightNeighborId
                    }).ToArray()
            };

            currentMatchService?.setPuzzle(puzzleManagerDto);
            OnGameStartedNavigation?.Invoke();
        }

        public void updateLobbyState(LobbyStateDto lobbyStateDto)
        {
            if (currentMatchService != null && lobbyStateDto != null)
            {
                var playerList = lobbyStateDto.Players?.ToList() ?? new List<string>();
                currentMatchService.initializeMatch(
                    lobbyStateDto.LobbyId,
                    playerList,
                    lobbyStateDto.CurrentSettingsDto,
                    lobbyStateDto.PuzzleImagePath
                );
            }

            OnLobbyStateUpdatedEvent?.Invoke(lobbyStateDto);
        }

        public void onPieceDragStarted(int pieceId, string username) => PieceDragStartedHandler?.Invoke(pieceId, username);
        public void onPieceMoved(int pieceId, double x, double y, string username) => PieceMovedHandler?.Invoke(pieceId, x, y, username);
        public void onPieceDragReleased(int pieceId, string username) => PieceDragReleasedHandler?.Invoke(pieceId, username);
        public void onPiecePlaced(int pieceId, double x, double y, string username, int score) => PiecePlacedHandler?.Invoke(pieceId, x, y, username, score);
    }
}