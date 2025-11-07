using MindWeaveClient.SocialManagerService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MindWeaveClient.Services.Abstractions
{
    public interface ISocialService
    {
        Task<FriendDto[]> getFriendsListAsync(string username);

        event Action<string, string> OnLobbyInviteReceived;

        bool connect(string username);

        void disconnect();
    }
}
