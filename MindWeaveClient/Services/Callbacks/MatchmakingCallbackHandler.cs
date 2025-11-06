using System.Collections.Generic;
using System.Linq;
using System.Windows;
using MindWeaveClient.MatchmakingService;

namespace MindWeaveClient.Services.Callbacks
{
    public class MatchmakingCallbackHandler : IMatchmakingManagerCallback
    {

        public void updateLobbyState(LobbyStateDto lobbyStateDto)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                GameEventAggregator.raiseLobbyStateUpdated(lobbyStateDto);
            });
        }

        public void lobbyCreationFailed(string reason)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                GameEventAggregator.raiseLobbyJoinFailed(reason);
            });
        }

        public void kickedFromLobby(string reason)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                GameEventAggregator.raiseLobbyJoinFailed(reason);
            });
        }

        public void matchFound(string matchId, string[] players)
        {
            List<string> playerList = players?.ToList() ?? new List<string>();
            Application.Current.Dispatcher.Invoke(() =>
            {
                GameEventAggregator.raiseMatchFound(matchId, playerList);
            });
        }

    }
}