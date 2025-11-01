using MindWeaveClient.ChatManagerService;
using MindWeaveClient.Services; 
using System;
using System.Windows;
using System.Windows.Input;

namespace MindWeaveClient.ViewModel.Game
{
    public class ChatMessageDisplayViewModel : BaseViewModel
    {
        public string SenderUsername { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        public HorizontalAlignment Alignment { get; set; }
        public ICommand ReportMessageCommand { get; }

        public ChatMessageDisplayViewModel(ChatMessageDto dto)
        {
            SenderUsername = dto.senderUsername;
            Content = dto.content;
            Timestamp = dto.timestamp.ToLocalTime();
            Alignment = dto.senderUsername.Equals(SessionService.Username, StringComparison.OrdinalIgnoreCase)
                            ? HorizontalAlignment.Right
                            : HorizontalAlignment.Left;
            ReportMessageCommand = new RelayCommand(param => executeReportMessage(), param => canExecuteReportMessage());
        }

        private bool canExecuteReportMessage() => SenderUsername != SessionService.Username;

        private void executeReportMessage()
        {
            MessageBox.Show($"Report button clicked for message from {SenderUsername}:\n'{Content}'\n(Functionality not implemented yet)", "Report Message");
        }
    }
}