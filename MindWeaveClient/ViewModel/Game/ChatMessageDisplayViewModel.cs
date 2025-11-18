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
            SenderUsername = dto.SenderUsername;
            Content = dto.Content;
            Timestamp = dto.Timestamp.ToLocalTime();
            Alignment = dto.SenderUsername.Equals(SessionService.Username, StringComparison.OrdinalIgnoreCase)
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