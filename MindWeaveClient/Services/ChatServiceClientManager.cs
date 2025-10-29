using MindWeaveClient.ChatManagerService;
using System;
using System.ServiceModel;

namespace MindWeaveClient.Services
{
    public sealed class ChatServiceClientManager
    {
        private static readonly Lazy<ChatServiceClientManager> lazy =
            new Lazy<ChatServiceClientManager>(() => new ChatServiceClientManager());

        public static ChatServiceClientManager Instance { get { return lazy.Value; } }

        public ChatManagerClient Proxy { get; private set; }
        public ChatCallbackHandler CallbackHandler { get; private set; }
        private InstanceContext site;
        private string connectedLobbyId = null;
        private string connectedUsername = null;


        private ChatServiceClientManager() { }

        public bool Connect(string username, string lobbyId)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(lobbyId))
            {
                Console.WriteLine("[Chat Connect] Error: Username and LobbyId required.");
                return false;
            }

            if (Proxy != null && Proxy.State == CommunicationState.Opened && connectedUsername == username && connectedLobbyId == lobbyId)
            {
                Console.WriteLine($"[Chat Connect] Already connected for User: {username}, Lobby: {lobbyId}.");
                return true;
            }

            if (Proxy != null)
            {
                Console.WriteLine($"[Chat Connect] Disconnecting existing chat connection (State: {Proxy.State}, User: {connectedUsername}, Lobby: {connectedLobbyId}) before reconnecting.");
                Disconnect();
            }


            try
            {
                Console.WriteLine($"[Chat Connect] Attempting connection for User: {username}, Lobby: {lobbyId}");
                CallbackHandler = new ChatCallbackHandler();
                site = new InstanceContext(CallbackHandler);
                Proxy = new ChatManagerClient(site, "NetTcpBinding_IChatManager");

                Proxy.Open();
                Console.WriteLine("[Chat Connect] WCF Channel Opened.");

                connectedUsername = username;
                connectedLobbyId = lobbyId;

                Proxy.joinLobbyChatAsync(username, lobbyId);
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
            Console.WriteLine($"[Chat Disconnect] Disconnecting User: {userToDisconnect}, Lobby: {lobbyToDisconnect}, Proxy State: {Proxy?.State}");


            if (Proxy != null && Proxy.State == CommunicationState.Opened && !string.IsNullOrEmpty(userToDisconnect) && !string.IsNullOrEmpty(lobbyToDisconnect))
            {
                Proxy.leaveLobbyChatAsync(userToDisconnect, lobbyToDisconnect); // Use Async for OneWay
                Console.WriteLine($"[Chat Disconnect] leaveLobbyChatAsync('{userToDisconnect}', '{lobbyToDisconnect}') called.");
            }

            try
            {
                if (Proxy != null)
                {
                    if (Proxy.State == CommunicationState.Opened || Proxy.State == CommunicationState.Opening) Proxy.Close();
                    else Proxy.Abort();
                    Console.WriteLine($"[Chat Disconnect] WCF Channel Closed/Aborted.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Chat Disconnect] Exception during channel close/abort: {ex.Message}");
                Proxy?.Abort();
            }
            finally
            {
                Proxy = null;
                site = null;
                CallbackHandler = null;
                connectedUsername = null;
                connectedLobbyId = null;
            }
        }

        public bool IsConnected()
        {
            return Proxy != null && Proxy.State == CommunicationState.Opened;
        }
    }
}