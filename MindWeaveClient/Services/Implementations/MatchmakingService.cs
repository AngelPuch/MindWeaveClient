using MindWeaveClient.MatchmakingService;
using MindWeaveClient.Properties.Langs;
using MindWeaveClient.Services.Abstractions;
using System;
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
                return new GuestJoinServiceResultDto(errorResult, false);
            }

            var matchmakingClient = MatchmakingServiceClientManager.instance.proxy;
            GuestJoinResultDto wcfResult = await matchmakingClient.joinLobbyAsGuestAsync(request);

            if (wcfResult.success && wcfResult.initialLobbyState != null)
            {
                SessionService.SetSession(wcfResult.assignedGuestUsername, null, true);
            }

            return new GuestJoinServiceResultDto(wcfResult, true);
        }

        public async Task joinLobbyAsync(string username, string lobbyCode)
        {
            if (!MatchmakingServiceClientManager.instance.EnsureConnected())
            {
                throw new InvalidOperationException(Lang.CannotConnectMatchmaking);
            }

            var matchmakingClient = MatchmakingServiceClientManager.instance.proxy;

            await Task.Run(() =>
            {
                matchmakingClient.joinLobby(username, lobbyCode);
            });
        }

        private void ensureConnected()
        {
            if (!MatchmakingServiceClientManager.instance.EnsureConnected())
            {
                throw new InvalidOperationException(Lang.CannotConnectMatchmaking);
            }
        }
        private MatchmakingManagerClient getProxy()
        {
            ensureConnected();
            return MatchmakingServiceClientManager.instance.proxy;
        }

        public async Task leaveLobbyAsync(string username, string lobbyId)
        {
            var proxy = getProxy();
            await Task.Run(() => proxy.leaveLobby(username, lobbyId));
        }

        public async Task startGameAsync(string hostUsername, string lobbyId)
        {
            var proxy = getProxy();
            await Task.Run(() => proxy.startGame(hostUsername, lobbyId));
        }

        public async Task kickPlayerAsync(string hostUsername, string playerToKick, string lobbyId)
        {
            var proxy = getProxy();
            await Task.Run(() => proxy.kickPlayer(hostUsername, playerToKick, lobbyId));
        }

        public async Task inviteToLobbyAsync(string inviter, string invited, string lobbyId)
        {
            var proxy = getProxy();
            await Task.Run(() => proxy.inviteToLobby(inviter, invited, lobbyId));
        }

        public async Task inviteGuestByEmailAsync(GuestInvitationDto invitationData)
        {
            // Este SÍ es async en el contrato del server, no necesita Task.Run
            await getProxy().inviteGuestByEmailAsync(invitationData);
        }

        public async Task changeDifficultyAsync(string hostUsername, string lobbyId, int newDifficultyId)
        {
            var proxy = getProxy();
            await Task.Run(() => proxy.changeDifficulty(hostUsername, lobbyId, newDifficultyId));
        }

    }
}
