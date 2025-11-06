using System.Windows;
using MindWeaveClient.ChatManagerService;

namespace MindWeaveClient.Services.Callbacks
{
    public class ChatCallbackHandler : IChatManagerCallback
    {
        public void receiveLobbyMessage(ChatMessageDto message)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                GameEventAggregator.raiseChatMessageReceived(message);
            });
        }
    }
}