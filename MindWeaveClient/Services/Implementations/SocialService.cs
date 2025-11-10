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
        private string currentUsername;

        public event Action<string, bool> FriendStatusChanged;
        public event Action<string> FriendRequestReceived;
        public event Action<string, bool> FriendResponseReceived;
        public event Action<string, string> LobbyInviteReceived;

        public async Task connectAsync(string username)
        {
            // Si ya estamos conectados con el mismo usuario, no hacer nada
            if (proxy != null &&
                proxy.State == CommunicationState.Opened &&
                currentUsername == username)
            {
                return;
            }

            // Si existe un proxy en cualquier otro estado, limpiarlo primero
            if (proxy != null)
            {
                cleanupProxy();
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
                currentUsername = username;
            }
            catch (Exception ex)
            {
                cleanupProxy();
                throw new InvalidOperationException("Failed to connect to social service.", ex);
            }
        }

        public async Task disconnectAsync(string username)
        {
            if (proxy == null || proxy.State != CommunicationState.Opened)
            {
                cleanupProxy();
                return;
            }

            try
            {
                await proxy.disconnectAsync(username);
                proxy.Close();
            }
            catch (Exception)
            {
                proxy?.Abort();
            }
            finally
            {
                cleanupProxy();
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
            catch (CommunicationException ex)
            {
                cleanupProxy();
                throw new InvalidOperationException("Lost connection to social service.", ex);
            }
            catch (TimeoutException ex)
            {
                cleanupProxy();
                throw new InvalidOperationException("Social service request timed out.", ex);
            }
        }

        private void cleanupProxy()
        {
            if (callbackHandler != null)
            {
                callbackHandler.FriendStatusChanged -= onFriendStatusChanged;
                callbackHandler.FriendRequestReceived -= onFriendRequestReceived;
                callbackHandler.FriendResponseReceived -= onFriendResponseReceived;
                callbackHandler.LobbyInviteReceived -= onLobbyInviteReceived;
            }

            if (proxy != null)
            {
                try
                {
                    if (proxy.State != CommunicationState.Closed &&
                        proxy.State != CommunicationState.Closing)
                    {
                        proxy.Abort();
                    }
                }
                catch
                {
                    // Ignorar errores al abortar
                }
            }

            proxy = null;
            callbackHandler = null;
            instanceContext = null;
            currentUsername = null;
        }
    }
}