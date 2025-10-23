// MindWeaveClient/ViewModel/Game/ChatMessageDisplayViewModel.cs
using MindWeaveClient.ChatManagerService;
using MindWeaveClient.Services; // Asegúrate que este using esté
using MindWeaveClient.ViewModel; // Asegúrate que este using esté (para BaseViewModel)
using System;
using System.Windows;
using System.Windows.Input;

namespace MindWeaveClient.ViewModel.Game // *** ASEGÚRATE QUE ESTÉ DENTRO DE ESTE NAMESPACE ***
{
    public class ChatMessageDisplayViewModel : BaseViewModel // Hereda de BaseViewModel
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