using MindWeaveClient.MatchmakingService;
using MindWeaveClient.Services.Abstractions;
using System;
using System.Collections.Generic;

namespace MindWeaveClient.Services.Implementations
{
    public class CurrentMatchService : ICurrentMatchService
    {
        private string matchId;
        private string lobbyId;
        private List<string> players;
        private LobbySettingsDto currentSettings;
        private string puzzleImagePath;
        private PuzzleManagerService.PuzzleDefinitionDto currentPuzzle;

        public event EventHandler OnMatchFound;
        public event Action PuzzleReady;

        public Guid MatchId { get; set; }
        public bool IsMatchActive => MatchId != Guid.Empty;
        public string LobbyId => lobbyId;
        public List<string> Players => players;
        public LobbySettingsDto CurrentSettings => currentSettings;
        public string PuzzleImagePath => puzzleImagePath;

        public void initializeMatch(string lobbyId, List<string> players, LobbySettingsDto settings, string puzzleImagePath)
        {
            this.lobbyId = lobbyId;
            this.players = players;
            this.currentSettings = settings;
            this.puzzleImagePath = puzzleImagePath;
            this.matchId = lobbyId;
            OnMatchFound?.Invoke(this, EventArgs.Empty);
        }

        public PuzzleManagerService.PuzzleDefinitionDto getCurrentPuzzle()
        {
            return currentPuzzle;
        }

        public void setPuzzle(PuzzleManagerService.PuzzleDefinitionDto puzzle)
        {
            this.currentPuzzle = puzzle;
            PuzzleReady?.Invoke();
        }

        public void setMatchId(string matchId)
        {
            this.matchId = matchId;
            if (string.IsNullOrEmpty(lobbyId))
            {
                this.lobbyId = matchId;
            }
        }

        public string getMatchId()
        {
            return matchId;
        }

        public void clearMatchData()
        {
            matchId = null;
            lobbyId = null;
            players = null;
            currentSettings = null;
            puzzleImagePath = null;
            currentPuzzle = null;
            MatchId = Guid.Empty;
        }
    }
}