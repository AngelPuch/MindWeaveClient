using MindWeaveClient.ChatManagerService;
using MindWeaveClient.Properties.Langs;
using MindWeaveClient.Services.Abstractions;
using MindWeaveClient.Services.Callbacks;
using System;
using System.ServiceModel;
using System.Threading.Tasks;

namespace MindWeaveClient.Services.Implementations
{
    public class ChatService : IChatService
    {
        private ChatManagerClient proxy;
        private ChatCallbackHandler callbackHandler;
        private InstanceContext instanceContext;

        private string connectedUsername;
        private string connectedLobbyId;

        public event Action<ChatMessageDto> OnMessageReceived;

        public bool isConnected()
        {
            return proxy != null && proxy.State == CommunicationState.Opened;
        }

        public async Task connectAsync(string username, string lobbyId)
        {
            if (isConnected())
            {
                if (connectedUsername == username && connectedLobbyId == lobbyId)
                {
                    return;
                }

                await disconnectAsync();
            }

            if (proxy != null)
            {
                proxy.Abort();
            }

            try
            {
                callbackHandler = new ChatCallbackHandler();
                instanceContext = new InstanceContext(callbackHandler);
                proxy = new ChatManagerClient(instanceContext);  

                callbackHandler.OnMessageReceivedEvent += handleMessageReceived;

                await Task.Run(() => proxy.joinLobbyChat(username, lobbyId));

                connectedUsername = username;
                connectedLobbyId = lobbyId;
            }
            catch (Exception ex)
            {
                if (proxy != null)
                {
                    proxy.Abort();
                    proxy = null;
                }
                connectedUsername = null;
                connectedLobbyId = null;
                throw;
            }
        }

        public async Task disconnectAsync()
        {
            if (!string.IsNullOrEmpty(connectedUsername) && !string.IsNullOrEmpty(connectedLobbyId))
            {
                await disconnectAsync(connectedUsername, connectedLobbyId);
            }
        }

        public async Task disconnectAsync(string username, string lobbyId)
        {
            if (!isConnected())
            {
                return;
            }

            if (username != connectedUsername || lobbyId != connectedLobbyId)
            {
                return;
            }

            try
            {
                await Task.Run(() => proxy.leaveLobbyChat(username, lobbyId));
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
                    callbackHandler.OnMessageReceivedEvent -= handleMessageReceived;
                }
                proxy = null;
                callbackHandler = null;
                instanceContext = null;
                connectedUsername = null;
                connectedLobbyId = null;
            }
        }

        public async Task sendLobbyMessageAsync(string username, string lobbyId, string message)
        {
            if (!isConnected())
            {
                throw new InvalidOperationException(Lang.ChatConnectError);
            }

            try
            {
                await Task.Run(() => proxy.sendLobbyMessage(username, lobbyId, message));
            }
            catch (Exception ex)
            {
                proxy.Abort();
                proxy = null;
                connectedUsername = null;
                connectedLobbyId = null;
                throw;
            }
        }

        private void handleMessageReceived(ChatMessageDto message)
        {
            OnMessageReceived?.Invoke(message);
        }
    }
}