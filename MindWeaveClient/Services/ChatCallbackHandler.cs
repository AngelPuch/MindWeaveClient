using MindWeaveClient.ChatManagerService;
using System;
using System.Diagnostics;
using System.Windows;

namespace MindWeaveClient.Services
{
    public class ChatCallbackHandler : IChatManagerCallback
    {
        public event Action<ChatMessageDto> LobbyMessageReceived;

        public void receiveLobbyMessage(ChatMessageDto messageDto)
        {
            Debug.WriteLine($"---> Chat Callback: Message received from {messageDto.senderUsername}: '{messageDto.content}'");
            Application.Current?.Dispatcher?.Invoke(() =>
            {
                LobbyMessageReceived?.Invoke(messageDto);
            });
        }
    }
}