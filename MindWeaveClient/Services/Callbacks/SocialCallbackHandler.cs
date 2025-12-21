using System;
using System.ServiceModel;
using System.Windows;
using MindWeaveClient.SocialManagerService;

namespace MindWeaveClient.Services.Callbacks
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class SocialCallbackHandler : ISocialManagerCallback
    {
        public event Action<string> FriendRequestReceived;
        public event Action<string, bool> FriendResponseReceived;
        public event Action<string, bool> FriendStatusChanged;
        public event Action<string, string> LobbyInviteReceived;

        public void notifyFriendRequest(string fromUsername)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                FriendRequestReceived?.Invoke(fromUsername);
            });
        }

        public void notifyFriendResponse(string fromUsername, bool accepted)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                FriendResponseReceived?.Invoke(fromUsername, accepted);
            });
        }

        public void notifyFriendStatusChanged(string friendUsername, bool isOnline)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                FriendStatusChanged?.Invoke(friendUsername, isOnline);
            });
        }

        public void notifyLobbyInvite(string fromUsername, string lobbyId)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                LobbyInviteReceived?.Invoke(fromUsername, lobbyId);
            });
        }

    }
}