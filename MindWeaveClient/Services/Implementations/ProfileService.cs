using MindWeaveClient.ProfileService;
using MindWeaveClient.Services.Abstractions;
using System;
using System.Threading.Tasks;

namespace MindWeaveClient.Services.Implementations
{
    public class ProfileService : IProfileService
    {
        public async Task<PlayerProfileViewDto> getPlayerProfileViewAsync(string username)
        {
            return await executeSafeAsync(async (client) =>
                await client.getPlayerProfileViewAsync(username));
        }

        public async Task<UserProfileForEditDto> getPlayerProfileForEditAsync(string username)
        {
            return await executeSafeAsync(async (client) =>
                await client.getPlayerProfileForEditAsync(username));
        }

        public async Task<OperationResultDto> updateProfileAsync(string username, UserProfileForEditDto updatedProfile)
        {
            return await executeSafeAsync(async (client) =>
                await client.updateProfileAsync(username, updatedProfile));
        }

        public async Task<OperationResultDto> changePasswordAsync(string username, string currentPassword, string newPassword)
        {
            return await executeSafeAsync(async (client) =>
                await client.changePasswordAsync(username, currentPassword, newPassword));
        }

        public async Task<OperationResultDto> updateAvatarPathAsync(string username, string avatarPath)
        {
            return await executeSafeAsync(async (client) =>
                await client.updateAvatarPathAsync(username, avatarPath));
        }
        private async Task<T> executeSafeAsync<T>(Func<ProfileManagerClient, Task<T>> action)
        {
            var client = new ProfileManagerClient();
            try
            {
                T result = await action(client);
                client.Close();
                return result;
            }
            catch (Exception)
            {
                client.Abort();
                throw;
            }
        }
    }
}