using System;
using System.Diagnostics;
using System.Windows;
using MindWeaveClient.SocialManagerService;

namespace MindWeaveClient.Services.Callbacks
{
    public class SocialCallbackHandler : ISocialManagerCallback
    {
        public event Action<string> FriendRequestReceived;
        public event Action<string, bool> FriendResponseReceived;
        public event Action<string, bool> FriendStatusChanged;
        public event Action<string, string> LobbyInviteReceived;

        public void notifyFriendRequest(string fromUsername)
        {
            Debug.WriteLine($"Callback: Friend request received from {fromUsername}");
            Application.Current.Dispatcher.Invoke(() =>
            {
                FriendRequestReceived?.Invoke(fromUsername);
            });
        }

        public void notifyFriendResponse(string fromUsername, bool accepted)
        {
            Debug.WriteLine($"Callback: Friend response from {fromUsername}. Accepted: {accepted}");
            Application.Current.Dispatcher.Invoke(() =>
            {
                FriendResponseReceived?.Invoke(fromUsername, accepted);
            });
        }

        public void notifyFriendStatusChanged(string friendUsername, bool isOnline)
        {
            Debug.WriteLine($"Callback: Friend status changed for {friendUsername}. Online: {isOnline}");
            Application.Current.Dispatcher.Invoke(() =>
            {
                FriendStatusChanged?.Invoke(friendUsername, isOnline);
            });
        }

        public void notifyLobbyInvite(string fromUsername, string lobbyId)
        {
            Debug.WriteLine($"Callback: Lobby invite received from {fromUsername} for lobby {lobbyId}");
            Application.Current.Dispatcher.Invoke(() =>
            {
                LobbyInviteReceived?.Invoke(fromUsername, lobbyId);
            });
        }

    }
}