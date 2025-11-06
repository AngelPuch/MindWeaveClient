using MindWeaveClient.ChatManagerService;
using System;
using System.ServiceModel;
using MindWeaveClient.Services.Callbacks;

namespace MindWeaveClient.Services
{
    public sealed class ChatServiceClientManager
    {
        private static readonly Lazy<ChatServiceClientManager> lazy =
            new Lazy<ChatServiceClientManager>(() => new ChatServiceClientManager());

        public static ChatServiceClientManager instance { get { return lazy.Value; } }

        public ChatManagerClient proxy { get; private set; }
        public ChatCallbackHandler callbackHandler { get; private set; }
        private InstanceContext site;
        private string connectedLobbyId;
        private string connectedUsername;


        private ChatServiceClientManager() { }

        public bool Connect(string username, string lobbyId)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(lobbyId))
            {
                Console.WriteLine("[Chat Connect] Error: Username and LobbyId required.");
                return false;
            }

            if (proxy != null && proxy.State == CommunicationState.Opened && connectedUsername == username && connectedLobbyId == lobbyId)
            {
                Console.WriteLine($"[Chat Connect] Already connected for User: {username}, Lobby: {lobbyId}.");
                return true;
            }

            if (proxy != null)
            {
                Console.WriteLine($"[Chat Connect] Disconnecting existing chat connection (State: {proxy.State}, User: {connectedUsername}, Lobby: {connectedLobbyId}) before reconnecting.");
                Disconnect();
            }


            try
            {
                Console.WriteLine($"[Chat Connect] Attempting connection for User: {username}, Lobby: {lobbyId}");
                callbackHandler = new ChatCallbackHandler();
                site = new InstanceContext(callbackHandler);
                proxy = new ChatManagerClient(site, "NetTcpBinding_IChatManager");

                proxy.Open();
                Console.WriteLine("[Chat Connect] WCF Channel Opened.");

                connectedUsername = username;
                connectedLobbyId = lobbyId;

                proxy.joinLobbyChatAsync(username, lobbyId);
                Console.WriteLine($"[Chat Connect] joinLobbyChatAsync('{username}', '{lobbyId}') called.");

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Chat Connect] Error connecting: {ex.Message}");
                Disconnect(); // Clean up on error
                return false;
            }
        }

        public void Disconnect()
        {
            string userToDisconnect = connectedUsername;
            string lobbyToDisconnect = connectedLobbyId;
            Console.WriteLine($"[Chat Disconnect] Disconnecting User: {userToDisconnect}, Lobby: {lobbyToDisconnect}, Proxy State: {proxy?.State}");


            if (proxy != null && proxy.State == CommunicationState.Opened && !string.IsNullOrEmpty(userToDisconnect) && !string.IsNullOrEmpty(lobbyToDisconnect))
            {
                proxy.leaveLobbyChatAsync(userToDisconnect, lobbyToDisconnect); // Use Async for OneWay
                Console.WriteLine($"[Chat Disconnect] leaveLobbyChatAsync('{userToDisconnect}', '{lobbyToDisconnect}') called.");
            }

            try
            {
                if (proxy != null)
                {
                    if (proxy.State == CommunicationState.Opened || proxy.State == CommunicationState.Opening) proxy.Close();
                    else proxy.Abort();
                    Console.WriteLine($"[Chat Disconnect] WCF Channel Closed/Aborted.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Chat Disconnect] Exception during channel close/abort: {ex.Message}");
                proxy?.Abort();
            }
            finally
            {
                proxy = null;
                site = null;
                callbackHandler = null;
                connectedUsername = null;
                connectedLobbyId = null;
            }
        }

        public bool IsConnected()
        {
            return proxy != null && proxy.State == CommunicationState.Opened;
        }
    }
}