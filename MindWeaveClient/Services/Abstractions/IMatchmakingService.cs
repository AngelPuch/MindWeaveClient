using MindWeaveClient.MatchmakingService;
using System.Threading.Tasks;

namespace MindWeaveClient.Services.Abstractions
{
    public interface IMatchmakingService
    {
        Task<GuestJoinServiceResultDto> joinLobbyAsGuestAsync(GuestJoinRequestDto request);
    }
}
