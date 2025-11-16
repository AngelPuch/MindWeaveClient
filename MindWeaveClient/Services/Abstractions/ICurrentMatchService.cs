using MindWeaveClient.MatchmakingService;
using System;
using System.Collections.Generic;

namespace MindWeaveClient.Services.Abstractions
{
    public interface ICurrentMatchService
    {
        string LobbyId { get; }
        List<string> Players { get; }
        LobbySettingsDto CurrentSettings { get; }
        string PuzzleImagePath { get; }

        event EventHandler OnMatchFound;

        void initializeMatch(string lobbyId, List<string> players, LobbySettingsDto settings, string puzzleImagePath);

        PuzzleManagerService.PuzzleDefinitionDto getCurrentPuzzle();
        void setPuzzle(PuzzleManagerService.PuzzleDefinitionDto puzzle);
        void setMatchId(string matchId); 
        string getMatchId();
    }
}