using MindWeaveClient.MatchmakingService;
using MindWeaveClient.Properties.Langs;
using MindWeaveClient.Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MindWeaveClient.Services.Implementations
{
    public class MatchmakingService : IMatchmakingService
    {

        public async Task<GuestJoinServiceResultDto> joinLobbyAsGuestAsync(GuestJoinRequestDto request)
        {
            if (!MatchmakingServiceClientManager.instance.EnsureConnected())
            {
                var errorResult = new GuestJoinResultDto
                {
                    success = false,
                    message = Lang.CannotConnectMatchmaking
                };
                return new GuestJoinServiceResultDto(errorResult, false, false);
            }

            var matchmakingClient = MatchmakingServiceClientManager.instance.proxy;
            GuestJoinResultDto wcfResult = await matchmakingClient.joinLobbyAsGuestAsync(request);

            bool socialConnected = false;

            if (wcfResult.success && wcfResult.initialLobbyState != null)
            {
                SessionService.SetSession(wcfResult.assignedGuestUsername, null, true);

                try
                {
                    socialConnected = SocialServiceClientManager.instance.Connect(wcfResult.assignedGuestUsername);
                }
                catch (Exception)
                {
                    socialConnected = false;
                }
            }

            return new GuestJoinServiceResultDto(wcfResult, socialConnected, true);
        }

    }
}
