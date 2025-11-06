using System;
using System.Collections.Generic;
using MindWeaveClient.ChatManagerService;
using MindWeaveClient.MatchmakingService;

namespace MindWeaveClient.Services.Callbacks
{
    public static class GameEventAggregator
    {
        public static event Action<string> OnLobbyJoinFailed;
        public static event Action<LobbyStateDto> OnLobbyStateUpdated;
        public static event Action<string, List<string>> OnMatchFound;
        public static event Action<ChatMessageDto> OnChatMessageReceived;

        public static void raiseLobbyJoinFailed(string reason)
        {
            OnLobbyJoinFailed?.Invoke(reason);
        }

        public static void raiseLobbyStateUpdated(LobbyStateDto lobbyState)
        {
            OnLobbyStateUpdated?.Invoke(lobbyState);
        }

        public static void raiseMatchFound(string matchId, List<string> players)
        {
            OnMatchFound?.Invoke(matchId, players);
        }
        public static void raiseChatMessageReceived(ChatMessageDto message)
        {
            OnChatMessageReceived?.Invoke(message);
        }
    }
}
