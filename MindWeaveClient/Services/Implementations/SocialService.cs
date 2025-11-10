using MindWeaveClient.Services.Abstractions;
using MindWeaveClient.Services.Callbacks;
using MindWeaveClient.SocialManagerService;
using System;
using System.ServiceModel;
using System.Threading.Tasks;

namespace MindWeaveClient.Services.Implementations
{
    public class SocialService : ISocialService
    {
        private SocialManagerClient proxy;
        private SocialCallbackHandler callbackHandler;
        private InstanceContext instanceContext;

        public event Action<string, bool> FriendStatusChanged;
        public event Action<string> FriendRequestReceived;
        public event Action<string, bool> FriendResponseReceived;
        public event Action<string, string> LobbyInviteReceived;

        public async Task connectAsync(string username)
        {
            if (proxy != null && proxy.State == CommunicationState.Opened)
            {
                return;
            }

            if (proxy != null)
            {
                proxy.Abort();
            }

            try
            {
                callbackHandler = new SocialCallbackHandler();
                instanceContext = new InstanceContext(callbackHandler);
                proxy = new SocialManagerClient(instanceContext);

                callbackHandler.FriendStatusChanged += onFriendStatusChanged;
                callbackHandler.FriendRequestReceived += onFriendRequestReceived;
                callbackHandler.FriendResponseReceived += onFriendResponseReceived;
                callbackHandler.LobbyInviteReceived += onLobbyInviteReceived;

                await proxy.connectAsync(username);
            }
            catch (Exception ex)
            {
                if (proxy != null)
                {
                    proxy.Abort();
                    proxy = null;
                }
                throw;
            }
        }

        public async Task disconnectAsync(string username)
        {
            if (proxy == null || proxy.State != CommunicationState.Opened)
            {
                return;
            }

            try
            {
                await proxy.disconnectAsync(username);
                proxy.Close();
            }
            catch (Exception ex)
            {
                proxy.Abort();
            }
            finally
            {
                if (callbackHandler != null)
                {
                    callbackHandler.FriendStatusChanged -= onFriendStatusChanged;
                    callbackHandler.FriendRequestReceived -= onFriendRequestReceived;
                    callbackHandler.FriendResponseReceived -= onFriendResponseReceived;
                    callbackHandler.LobbyInviteReceived -= onLobbyInviteReceived;
                }
                proxy = null;
                callbackHandler = null;
                instanceContext = null;
            }
        }
        public async Task<FriendDto[]> getFriendsListAsync(string username)
        {
            return await executeSafeAsync(async () => await proxy.getFriendsListAsync(username));
        }
        public async Task<FriendRequestInfoDto[]> getFriendRequestsAsync(string username)
        {
            return await executeSafeAsync(async () => await proxy.getFriendRequestsAsync(username));
        }
        public async Task<PlayerSearchResultDto[]> searchPlayersAsync(string username, string query)
        {
            return await executeSafeAsync(async () => await proxy.searchPlayersAsync(username, query));
        }
        public async Task<OperationResultDto> sendFriendRequestAsync(string username, string targetUsername)
        {
            return await executeSafeAsync(async () => await proxy.sendFriendRequestAsync(username, targetUsername));
        }
        public async Task<OperationResultDto> respondToFriendRequestAsync(string username, string requesterUsername, bool accept)
        {
            return await executeSafeAsync(async () => await proxy.respondToFriendRequestAsync(username, requesterUsername, accept));
        }
        public async Task<OperationResultDto> removeFriendAsync(string username, string friendUsername)
        {
            return await executeSafeAsync(async () => await proxy.removeFriendAsync(username, friendUsername));
        }
        private void onFriendStatusChanged(string friendUsername, bool isOnline)
        {
            FriendStatusChanged?.Invoke(friendUsername, isOnline);
        }

        private void onFriendRequestReceived(string fromUsername)
        {
            FriendRequestReceived?.Invoke(fromUsername);
        }

        private void onFriendResponseReceived(string fromUsername, bool accepted)
        {
            FriendResponseReceived?.Invoke(fromUsername, accepted);
        }

        private void onLobbyInviteReceived(string fromUsername, string lobbyCode)
        {
            LobbyInviteReceived?.Invoke(fromUsername, lobbyCode);
        }

        private async Task<T> executeSafeAsync<T>(Func<Task<T>> call)
        {
            if (proxy == null || proxy.State != CommunicationState.Opened)
            {
                throw new InvalidOperationException("Social service is not connected.");
            }
            try
            {
                return await call();
            }
            catch (Exception ex)
            {
                proxy.Abort();
                proxy = null;
                throw new InvalidOperationException("Lost connection to social service.", ex);
            }
        }
    }
}