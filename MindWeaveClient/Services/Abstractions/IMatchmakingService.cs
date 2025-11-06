using MindWeaveClient.MatchmakingService;
using System.Threading.Tasks;

namespace MindWeaveClient.Services.Abstractions
{
    public interface IMatchmakingService
    {
        Task<GuestJoinServiceResultDto> joinLobbyAsGuestAsync(GuestJoinRequestDto request);
        Task joinLobbyAsync(string username, string lobbyCode);
        Task leaveLobbyAsync(string username, string lobbyId);
        Task startGameAsync(string hostUsername, string lobbyId);
        Task kickPlayerAsync(string hostUsername, string playerToKick, string lobbyId);
        Task inviteToLobbyAsync(string inviter, string invited, string lobbyId);
        Task inviteGuestByEmailAsync(GuestInvitationDto invitationData);
        Task changeDifficultyAsync(string hostUsername, string lobbyId, int newDifficultyId);
    }
}
