using MindWeaveClient.ChatManagerService;
using MindWeaveClient.Services;
using System;

namespace MindWeaveClient.ViewModel.Game
{
    public class ChatMessageDisplayViewModel : BaseViewModel
    {
        private const string RIGHT = "Right";
        private const string LEFT = "Left";

        private readonly ChatMessageDto dto;

        public ChatMessageDisplayViewModel(ChatMessageDto dto)
        {
            this.dto = dto ?? throw new ArgumentNullException(nameof(dto));
        }
        public string SenderUsername => dto.SenderUsername;
        public string Content => dto.Content;
        public string Time => dto.Timestamp.ToLocalTime().ToShortTimeString();     
        public bool IsMyMessage => dto.SenderUsername == SessionService.Username;
        public string Alignment => IsMyMessage ? RIGHT : LEFT;
    }
}