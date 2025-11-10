using MindWeaveClient.Services.Abstractions;
using MindWeaveClient.Services.Callbacks;
using MindWeaveClient.SocialManagerService;
using System;
using System.Diagnostics;
using System.ServiceModel;
using System.Threading.Tasks;

namespace MindWeaveClient.Services.Implementations
{
    /// <summary>
    /// Singleton implementation of ISocialService.
    /// Manages the WCF proxy, InstanceContext, and CallbackHandler lifetime
    /// for the SocialManager service.
    /// </summary>
    public class SocialService : ISocialService
    {
        private SocialManagerClient proxy;
        private SocialCallbackHandler callbackHandler;
        private InstanceContext instanceContext;

        // --- Eventos Públicos de la Interfaz ---
        public event Action<string, bool> FriendStatusChanged;
        public event Action<string> FriendRequestReceived;
        public event Action<string, bool> FriendResponseReceived;
        public event Action<string, string> LobbyInviteReceived; // <--- AÑADIDO

        public async Task connectAsync(string username)
        {
            if (proxy != null && proxy.State == CommunicationState.Opened)
            {
                return; // Already connected
            }

            if (proxy != null)
            {
                proxy.Abort(); // Clean up any faulted client
            }

            try
            {
                callbackHandler = new SocialCallbackHandler();
                instanceContext = new InstanceContext(callbackHandler);
                proxy = new SocialManagerClient(instanceContext);

                // Suscribirse a TODOS los eventos del handler de WCF
                callbackHandler.FriendStatusChanged += onFriendStatusChanged;
                callbackHandler.FriendRequestReceived += onFriendRequestReceived;
                callbackHandler.FriendResponseReceived += onFriendResponseReceived;
                callbackHandler.LobbyInviteReceived += onLobbyInviteReceived; // <--- AÑADIDO

                await proxy.connectAsync(username);
                Trace.TraceInformation("SocialService connected successfully.");
            }
            catch (Exception ex)
            {
                Trace.TraceError($"Failed to connect SocialService: {ex.Message}");
                if (proxy != null)
                {
                    proxy.Abort();
                    proxy = null;
                }
                throw; // Relanzar para que el ViewModel lo maneje
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
                Trace.TraceInformation("SocialService disconnected successfully.");
            }
            catch (Exception ex)
            {
                Trace.TraceWarning($"Unclean disconnect from SocialService: {ex.Message}");
                proxy.Abort();
            }
            finally
            {
                // Desuscribirse de TODOS los eventos
                if (callbackHandler != null)
                {
                    callbackHandler.FriendStatusChanged -= onFriendStatusChanged;
                    callbackHandler.FriendRequestReceived -= onFriendRequestReceived;
                    callbackHandler.FriendResponseReceived -= onFriendResponseReceived;
                    callbackHandler.LobbyInviteReceived -= onLobbyInviteReceived; // <--- AÑADIDO
                }
                proxy = null;
                callbackHandler = null;
                instanceContext = null;
            }
        }

        // --- Métodos de Operación (FriendList, Search, etc.) ---

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

        // --- Puentes de Callbacks (Handlers privados) ---

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
            LobbyInviteReceived?.Invoke(fromUsername, lobbyCode); // <--- AÑADIDO
        }

        // --- Helper de Seguridad de WCF ---

        private async Task<T> executeSafeAsync<T>(Func<Task<T>> call)
        {
            if (proxy == null || proxy.State != CommunicationState.Opened)
            {
                Trace.TraceError("Attempted to call SocialService while disconnected.");
                throw new InvalidOperationException("Social service is not connected.");
            }
            try
            {
                return await call();
            }
            catch (Exception ex)
            {
                Trace.TraceError($"WCF call failed: {ex.Message}. Aborting proxy.");
                proxy.Abort();
                proxy = null;
                throw new InvalidOperationException("Lost connection to social service.", ex);
            }
        }
    }
}