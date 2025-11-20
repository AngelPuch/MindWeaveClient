using System;
using MindWeaveClient.ChatManagerService;

namespace MindWeaveClient.Services.Callbacks
{
    public class ChatCallbackHandler : IChatManagerCallback
    {
        public event Action<ChatMessageDto> OnMessageReceivedEvent;

        public void receiveLobbyMessage(ChatMessageDto messageDto)
        {
            OnMessageReceivedEvent?.Invoke(messageDto);
        }
    }
}