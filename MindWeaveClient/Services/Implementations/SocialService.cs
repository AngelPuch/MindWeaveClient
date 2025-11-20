// ============================================
// 1. SocialService.cs - REEMPLAZAR COMPLETAMENTE
// ============================================
using MindWeaveClient.Services.Abstractions;
using MindWeaveClient.Services.Callbacks;
using MindWeaveClient.SocialManagerService;
using System;
using System.Diagnostics;
using System.ServiceModel;
using System.Threading.Tasks;

namespace MindWeaveClient.Services.Implementations
{
    public class SocialService : ISocialService
    {
        private SocialManagerClient proxy;
        private readonly SocialCallbackHandler callbackHandler;
        private string currentUsername;
        private readonly object lockObject = new object();

        public event Action<string, bool> FriendStatusChanged;
        public event Action<string> FriendRequestReceived;
        public event Action<string, bool> FriendResponseReceived;
        public event Action<string, string> LobbyInviteReceived;

        // Recibir el callback handler por inyección de dependencias
        public SocialService(ISocialManagerCallback callbackHandler)
        {
            this.callbackHandler = callbackHandler as SocialCallbackHandler;

            if (this.callbackHandler != null)
            {
                // Suscribirse a los eventos del callback handler
                this.callbackHandler.FriendStatusChanged += onFriendStatusChanged;
                this.callbackHandler.FriendRequestReceived += onFriendRequestReceived;
                this.callbackHandler.FriendResponseReceived += onFriendResponseReceived;
                this.callbackHandler.LobbyInviteReceived += onLobbyInviteReceived;
            }
        }

        public async Task connectAsync(string username)
        {
            lock (lockObject)
            {
                // Si ya está conectado con el mismo usuario, no hacer nada
                if (proxy != null &&
                    proxy.State == CommunicationState.Opened &&
                    currentUsername == username)
                {
                    Trace.TraceInformation($"Social service already connected for {username}");
                    return;
                }

                // Si hay un proxy anterior, limpiarlo
                if (proxy != null)
                {
                    Trace.TraceWarning($"Cleaning up previous proxy before reconnecting");
                    cleanupProxy(false);
                }
            }

            try
            {
                var instanceContext = new InstanceContext(callbackHandler);
                proxy = new SocialManagerClient(instanceContext);

                await proxy.connectAsync(username);
                currentUsername = username;

                Trace.TraceInformation($"Social service connected successfully for {username}");
            }
            catch (Exception ex)
            {
                Trace.TraceError($"Failed to connect social service: {ex.Message}");
                cleanupProxy(false);
                throw new InvalidOperationException("Failed to connect to social service.", ex);
            }
        }

        public async Task disconnectAsync(string username)
        {
            if (proxy == null || proxy.State != CommunicationState.Opened)
            {
                cleanupProxy(false);
                return;
            }

            try
            {
                Trace.TraceInformation($"Disconnecting social service for {username}");
                await proxy.disconnectAsync(username);
                proxy.Close();
            }
            catch (Exception ex)
            {
                Trace.TraceWarning($"Error during disconnect: {ex.Message}");
                proxy?.Abort();
            }
            finally
            {
                cleanupProxy(false);
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
            Trace.TraceInformation($"Friend status changed: {friendUsername} - {(isOnline ? "Online" : "Offline")}");
            FriendStatusChanged?.Invoke(friendUsername, isOnline);
        }

        private void onFriendRequestReceived(string fromUsername)
        {
            Trace.TraceInformation($"Friend request received from: {fromUsername}");
            FriendRequestReceived?.Invoke(fromUsername);
        }

        private void onFriendResponseReceived(string fromUsername, bool accepted)
        {
            Trace.TraceInformation($"Friend response received from {fromUsername}: {accepted}");
            FriendResponseReceived?.Invoke(fromUsername, accepted);
        }

        private void onLobbyInviteReceived(string fromUsername, string lobbyCode)
        {
            Trace.TraceInformation($"Lobby invite received from {fromUsername}: {lobbyCode}");
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
                Trace.TraceError($"Communication error in social service: {ex.Message}");
                cleanupProxy(false);
                throw;
            }
            catch (Exception ex)
            {
                Trace.TraceError($"Error in social service: {ex.Message}");
                throw;
            }
        }

        private void cleanupProxy(bool unsubscribeFromCallbacks)
        {
            // NO desuscribir de los eventos del callback handler
            // ya que es un singleton y debe mantener las suscripciones

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
                catch (Exception ex)
                {
                    Trace.TraceWarning($"Error aborting proxy: {ex.Message}");
                }
            }

            proxy = null;
            currentUsername = null;
        }

        public void Dispose()
        {
            Trace.TraceInformation("Disposing SocialService");

            if (callbackHandler != null)
            {
                callbackHandler.FriendStatusChanged -= onFriendStatusChanged;
                callbackHandler.FriendRequestReceived -= onFriendRequestReceived;
                callbackHandler.FriendResponseReceived -= onFriendResponseReceived;
                callbackHandler.LobbyInviteReceived -= onLobbyInviteReceived;
            }

            cleanupProxy(true);
        }
    }
}