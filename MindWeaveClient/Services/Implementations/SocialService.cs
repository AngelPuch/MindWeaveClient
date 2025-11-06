using MindWeaveClient.Services.Abstractions;
using MindWeaveClient.SocialManagerService;
using System;
using System.Threading.Tasks;

namespace MindWeaveClient.Services.Implementations
{
    public class SocialService : ISocialService
    {
        public async Task<FriendDto[]> getFriendsListAsync(string username)
        {
            if (!SocialServiceClientManager.instance.EnsureConnected(username))
            {
                throw new InvalidOperationException("Could not connect to Social service.");
            }

            var socialProxy = SocialServiceClientManager.instance.proxy;
            return await socialProxy.getFriendsListAsync(username);
        }
    }
}
