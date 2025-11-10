using System;
using System.Windows;
using MindWeaveClient.ChatManagerService;

namespace MindWeaveClient.Services.Callbacks
{
    public class ChatCallbackHandler : IChatManagerCallback
    {
        public event Action<ChatMessageDto> OnMessageReceivedEvent;

        public void receiveLobbyMessage(ChatMessageDto message)
        {
            OnMessageReceivedEvent?.Invoke(message);
        }
    }
}