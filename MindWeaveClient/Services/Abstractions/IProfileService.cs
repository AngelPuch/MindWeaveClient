using MindWeaveClient.ProfileService;
using System.Threading.Tasks;

namespace MindWeaveClient.Services.Abstractions
{
    public interface IProfileService
    {
        Task<PlayerProfileViewDto> getPlayerProfileViewAsync(string username);

        Task<UserProfileForEditDto> getPlayerProfileForEditAsync(string username);

        Task<OperationResultDto> updateProfileAsync(string username, UserProfileForEditDto updatedProfile);

        Task<OperationResultDto> changePasswordAsync(string username, string currentPassword, string newPassword);

        Task<OperationResultDto> updateAvatarPathAsync(string username, string avatarPath);
    }
}
