using MindWeaveClient.Services.Abstractions;
using MindWeaveClient.SocialManagerService;
using System;
using System.ServiceModel;
using System.Threading.Tasks;

namespace MindWeaveClient.Services.Implementations
{
    public class SocialService : ISocialService
    {
        public event Action<string, string> OnLobbyInviteReceived;
        public async Task<FriendDto[]> getFriendsListAsync(string username)
        {
            var proxy = getProxy();
            return await proxy.getFriendsListAsync(username);
        }

        public bool connect(string username)
        {
            bool connected = SocialServiceClientManager.instance.EnsureConnected(username);

            if (connected && SocialServiceClientManager.instance.callbackHandler != null)
            {
                SocialServiceClientManager.instance.callbackHandler.LobbyInviteReceived -= onLobbyInvite;
                SocialServiceClientManager.instance.callbackHandler.LobbyInviteReceived += onLobbyInvite;
            }
            return connected;
        }

        public void disconnect()
        {
            if (SocialServiceClientManager.instance.callbackHandler != null)
            {
                SocialServiceClientManager.instance.callbackHandler.LobbyInviteReceived -= onLobbyInvite;
            }
            SocialServiceClientManager.instance.Disconnect();
        }
        private SocialManagerClient getProxy()
        {
            if (SocialServiceClientManager.instance.proxy?.State != CommunicationState.Opened)
            {
                if (!string.IsNullOrEmpty(SessionService.Username))
                {
                    connect(SessionService.Username);
                }
            }
            return SocialServiceClientManager.instance.proxy;
        }

        private void onLobbyInvite(string fromUsername, string lobbyId)
        {
            OnLobbyInviteReceived?.Invoke(fromUsername, lobbyId);
        }
    }
}
