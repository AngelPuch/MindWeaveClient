using MindWeaveClient.ChatManagerService;
using MindWeaveClient.Services; 
using System;
using System.Windows;
using System.Windows.Input;

namespace MindWeaveClient.ViewModel.Game
{
    public class ChatMessageDisplayViewModel : BaseViewModel
    {
        public string senderUsername { get; set; }
        public string content { get; set; }
        public DateTime timestamp { get; set; }
        public HorizontalAlignment alignment { get; set; }
        public ICommand reportMessageCommand { get; }

        public ChatMessageDisplayViewModel(ChatMessageDto dto)
        {
            senderUsername = dto.senderUsername;
            content = dto.content;
            timestamp = dto.timestamp.ToLocalTime();
            alignment = dto.senderUsername.Equals(SessionService.username, StringComparison.OrdinalIgnoreCase)
                            ? HorizontalAlignment.Right
                            : HorizontalAlignment.Left;
            reportMessageCommand = new RelayCommand(param => executeReportMessage(), param => canExecuteReportMessage());
        }

        private bool canExecuteReportMessage() => senderUsername != SessionService.username;

        private void executeReportMessage()
        {
            MessageBox.Show($"Report button clicked for message from {senderUsername}:\n'{content}'\n(Functionality not implemented yet)", "Report Message");
        }
    }
}