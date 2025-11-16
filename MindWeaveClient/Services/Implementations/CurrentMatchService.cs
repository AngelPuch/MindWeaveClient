using MindWeaveClient.MatchmakingService;
using MindWeaveClient.Services.Abstractions;
using System;
using System.Collections.Generic;

namespace MindWeaveClient.Services.Implementations
{
    public class CurrentMatchService : ICurrentMatchService
    {
        private string matchId;
        private PuzzleManagerService.PuzzleDefinitionDto currentPuzzle;

        public string LobbyId { get; private set; }
        public List<string> Players { get; private set; }
        public LobbySettingsDto CurrentSettings { get; private set; }
        public string PuzzleImagePath { get; private set; }

        public event EventHandler OnMatchFound;
        public event Action PuzzleReady;

        public void initializeMatch(string lobbyId, List<string> players, LobbySettingsDto settings, string puzzleImagePath)
        {
            this.LobbyId = lobbyId;
            this.Players = players;
            this.CurrentSettings = settings;
            this.PuzzleImagePath = puzzleImagePath;

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
            if (string.IsNullOrEmpty(this.LobbyId))
            {
                this.LobbyId = matchId;
            }
        }

        public string getMatchId()
        {
            var id = matchId;
            return id;
        }
    }
}