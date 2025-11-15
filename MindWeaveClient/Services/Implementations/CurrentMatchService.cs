using MindWeaveClient.MatchmakingService;
using MindWeaveClient.Services.Abstractions;
using System;
using System.Collections.Generic;

namespace MindWeaveClient.Services.Implementations
{
    public class CurrentMatchService : ICurrentMatchService
    {
        private string matchId;
        private PuzzleDefinitionDto currentPuzzle;

        public string LobbyId { get; private set; }
        public List<string> Players { get; private set; }
        public LobbySettingsDto CurrentSettings { get; private set; }
        public string PuzzleImagePath { get; private set; }

        public event EventHandler OnMatchFound;

        public void InitializeMatch(string lobbyId, List<string> players, LobbySettingsDto settings, string puzzleImagePath)
        {
            this.LobbyId = lobbyId;
            this.Players = players;
            this.CurrentSettings = settings;
            this.PuzzleImagePath = puzzleImagePath;

            this.matchId = lobbyId; 

            OnMatchFound?.Invoke(this, EventArgs.Empty);
        }

        public PuzzleDefinitionDto getCurrentPuzzle()
        {
            return currentPuzzle;
        }

        public void setPuzzle(PuzzleDefinitionDto puzzle)
        {
            this.currentPuzzle = puzzle;
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