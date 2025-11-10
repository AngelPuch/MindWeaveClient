using MindWeaveClient.ProfileService;
using MindWeaveClient.Services.Abstractions;
using System;
using System.ServiceModel;
using System.Threading.Tasks;

namespace MindWeaveClient.Services.Implementations
{
    public class ProfileService : IProfileService
    {
        public async Task<PlayerProfileViewDto> getPlayerProfileViewAsync(string username)
        {
            ProfileManagerClient client = new ProfileManagerClient();
            try
            {
                PlayerProfileViewDto profileData = await client.getPlayerProfileViewAsync(username);
                client.Close();
                return profileData;
            }
            catch (CommunicationException)
            {
                client.Abort();
                throw;
            }
            catch (TimeoutException)
            {
                client.Abort();
                throw;
            }
            catch
            {
                client.Abort();
                throw;
            }
        }

        public async Task<UserProfileForEditDto> getPlayerProfileForEditAsync(string username)
        {
            var client = new ProfileManagerClient();
            try
            {
                UserProfileForEditDto result = await client.getPlayerProfileForEditAsync(username);
                client.Close();
                return result;
            }
            catch (CommunicationException)
            {
                client.Abort();
                throw;
            }
            catch (TimeoutException)
            {
                client.Abort();
                throw;
            }
            catch
            {
                client.Abort();
                throw;
            }
        }

        public async Task<OperationResultDto> updateProfileAsync(string username, UserProfileForEditDto updatedProfile)
        {
            var client = new ProfileManagerClient();
            try
            {
                OperationResultDto result = await client.updateProfileAsync(username, updatedProfile);
                client.Close();
                return result;
            }
            catch (CommunicationException)
            {
                client.Abort();
                throw;
            }
            catch (TimeoutException)
            {
                client.Abort();
                throw;
            }
            catch
            {
                client.Abort();
                throw;
            }
        }

        public async Task<OperationResultDto> changePasswordAsync(string username, string currentPassword, string newPassword)
        {
            var client = new ProfileManagerClient();
            try
            {
                OperationResultDto result = await client.changePasswordAsync(username, currentPassword, newPassword);
                client.Close();
                return result;
            }
            catch (CommunicationException)
            {
                client.Abort();
                throw;
            }
            catch (TimeoutException)
            {
                client.Abort();
                throw;
            }
            catch
            {
                client.Abort();
                throw;
            }
        }

        public async Task<OperationResultDto> updateAvatarPathAsync(string username, string avatarPath)
        {
            var client = new ProfileManagerClient();
            try
            {
                OperationResultDto result = await client.updateAvatarPathAsync(username, avatarPath);
                client.Close();
                return result;
            }
            catch (CommunicationException)
            {
                client.Abort();
                throw;
            }
            catch (TimeoutException)
            {
                client.Abort();
                throw;
            }
            catch
            {
                client.Abort();
                throw;
            }
        }
    }
}
