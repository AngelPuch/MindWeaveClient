using MindWeaveClient.MatchmakingService; 
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace MindWeaveClient.Services
{
    public class MatchmakingCallbackHandler : IMatchmakingManagerCallback
    {

        public event Action<LobbyStateDto> LobbyStateUpdated;
        public event Action<string, List<string>> MatchFound;
        public event Action<string> LobbyCreationFailed;
        public event Action<string> KickedFromLobby;


        public void updateLobbyState(LobbyStateDto lobbyStateDto)
        {
            Debug.WriteLine($"Callback: Lobby state updated for lobby {lobbyStateDto?.lobbyId}");
            Application.Current.Dispatcher.Invoke(() => { LobbyStateUpdated?.Invoke(lobbyStateDto); });
        }

       
        public void matchFound(string matchId, string[] players)
        {
            List<string> playerList = players?.ToList() ?? new List<string>();
            Debug.WriteLine($"Callback: Match found! ID: {matchId}. Players: {string.Join(", ", playerList)}");
            Application.Current.Dispatcher.Invoke(() => { MatchFound?.Invoke(matchId, playerList); });
        }
       

        public void lobbyCreationFailed(string reason)
        {
            Debug.WriteLine($"Callback: Lobby creation failed. Reason: {reason}");
            Application.Current.Dispatcher.Invoke(() => { LobbyCreationFailed?.Invoke(reason); });
        }

        public void kickedFromLobby(string reason)
        {
            Debug.WriteLine($"Callback: Kicked from lobby. Reason: {reason}");
            Application.Current.Dispatcher.Invoke(() => { KickedFromLobby?.Invoke(reason); });
        }
    }
}