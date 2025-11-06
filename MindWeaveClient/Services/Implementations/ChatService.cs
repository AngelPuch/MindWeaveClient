using MindWeaveClient.Services.Abstractions;
using System;
using System.Threading.Tasks;

namespace MindWeaveClient.Services.Implementations
{
    public class ChatService : IChatService
    {
        public bool connect(string username, string lobbyId)
        {
            return ChatServiceClientManager.instance.Connect(username, lobbyId);
        }

        public void disconnect()
        {
            ChatServiceClientManager.instance.Disconnect();
        }

        public bool isConnected()
        {
            return ChatServiceClientManager.instance.IsConnected();
        }

        public async Task sendLobbyMessageAsync(string username, string lobbyId, string message)
        {
            if (!isConnected())
            {
                throw new InvalidOperationException("Not connected to chat service.");
            }

            var chatProxy = ChatServiceClientManager.instance.proxy;

            await Task.Run(() =>
            {
                chatProxy.sendLobbyMessage(username, lobbyId, message);
            });
        }
    }
}
