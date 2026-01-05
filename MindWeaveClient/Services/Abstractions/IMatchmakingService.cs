using MindWeaveClient.MatchmakingService;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MindWeaveClient.Services.Abstractions
{
    public interface IMatchmakingService
    {
        event Action<LobbyStateDto> OnLobbyStateUpdated;
        event Action<string, List<string>, LobbySettingsDto, string> OnMatchFound;
        event Action<string> OnLobbyCreationFailed;
        event Action<string> OnKicked;
        event Action<string> OnLobbyActionFailed;
        event Action<string> OnLobbyDestroyed;
        event Action<string, string> OnAchievementUnlocked;
        event Action<PuzzleDefinitionDto, int> OnGameStarted;

        Task<LobbyCreationResultDto> createLobbyAsync(string hostUsername, LobbySettingsDto settings);
        Task<GuestJoinServiceResultDto> joinLobbyAsGuestAsync(GuestJoinRequestDto request);
        Task<JoinLobbyResultDto> joinLobbyWithConfirmationAsync(string username, string lobbyCode);

        [Obsolete("Use joinLobbyWithConfirmationAsync instead for proper error handling")]
        Task joinLobbyAsync(string username, string lobbyCode);

        Task leaveLobbyAsync(string username, string lobbyId);
        Task startGameAsync(string hostUsername, string lobbyId);
        Task kickPlayerAsync(string hostUsername, string playerToKick, string lobbyId);
        Task inviteToLobbyAsync(string inviter, string invited, string lobbyId);
        Task inviteGuestByEmailAsync(GuestInvitationDto invitationData);
        Task changeDifficultyAsync(string hostUsername, string lobbyId, int newDifficultyId);
        void disconnect();
        Task requestPieceDragAsync(string lobbyCode, int pieceId);
        Task requestPieceMoveAsync(string lobbyCode, int pieceId, double newX, double newY);
        Task requestPieceDropAsync(string lobbyCode, int pieceId, double newX, double newY);
        Task requestPieceReleaseAsync(string lobbyCode, int pieceId);
        Task leaveGameAsync(string username, string lobbyCode);
    }
}