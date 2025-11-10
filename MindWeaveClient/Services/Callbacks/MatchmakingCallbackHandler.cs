using MindWeaveClient.MatchmakingService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace MindWeaveClient.Services.Callbacks
{
    [CallbackBehavior(UseSynchronizationContext = false)]
    public class MatchmakingCallbackHandler : IMatchmakingManagerCallback
    {
        public event Action<LobbyStateDto> OnLobbyStateUpdatedEvent;
        public event Action<string, List<string>> OnMatchFoundEvent;
        public event Action<string> OnLobbyCreationFailedEvent;
        public event Action<string> OnKickedEvent;

        public void updateLobbyState(LobbyStateDto lobbyStateDto)
        {
            OnLobbyStateUpdatedEvent?.Invoke(lobbyStateDto);
        }

        public void matchFound(string matchId, string[] players)
        {
            List<string> playerList = players?.ToList() ?? new List<string>();
            OnMatchFoundEvent?.Invoke(matchId, playerList);
        }

        public void lobbyCreationFailed(string reason)
        {
            OnLobbyCreationFailedEvent?.Invoke(reason);
        }

        public void kickedFromLobby(string reason)
        {
            OnKickedEvent?.Invoke(reason);
        }
    }
}