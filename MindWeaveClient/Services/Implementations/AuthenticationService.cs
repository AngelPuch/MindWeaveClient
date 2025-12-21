using MindWeaveClient.AuthenticationService;
using MindWeaveClient.Services.Abstractions;
using System;
using System.Threading.Tasks;

namespace MindWeaveClient.Services.Implementations
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly ISocialService socialService;

        public AuthenticationService(ISocialService socialService)
        {
            this.socialService = socialService;
        }

        public async Task<LoginServiceResultDto> loginAsync(string email, string password)
        {
            var loginCredentials = new LoginDto
            {
                Email = email,
                Password = password
            };

            LoginResultDto result = await executeSafeAsync(async (client) =>
                await client.loginAsync(loginCredentials));

            if (result.OperationResult.Success)
            {
                SessionService.setSession(result.PlayerId, result.Username, result.AvatarPath);
                bool socialConnected = await connectSocialServiceAsync(result.Username);

                return new LoginServiceResultDto(result, socialConnected, true);
            }
            else
            {
                return new LoginServiceResultDto(result);
            }
        }

        public async Task<OperationResultDto> resendVerificationCodeAsync(string email)
        {
            return await executeSafeAsync(async (client) =>
                await client.resendVerificationCodeAsync(email));
        }

        public async Task<OperationResultDto> registerAsync(UserProfileDto profile, string password)
        {
            return await executeSafeAsync(async (client) =>
                await client.registerAsync(profile, password));
        }

        public async Task<OperationResultDto> verifyAccountAsync(string email, string code)
        {
            return await executeSafeAsync(async (client) =>
                await client.verifyAccountAsync(email, code));
        }

        public async Task<OperationResultDto> sendPasswordRecoveryCodeAsync(string email)
        {
            return await executeSafeAsync(async (client) =>
                await client.sendPasswordRecoveryCodeAsync(email));
        }

        public async Task<OperationResultDto> resetPasswordWithCodeAsync(string email, string code, string newPassword)
        {
            return await executeSafeAsync(async (client) =>
                await client.resetPasswordWithCodeAsync(email, code, newPassword));
        }

        public async Task logoutAsync(string username)
        {
            await executeSafeAsync(async (client) =>
            {
                client.logOut(username);
                return 0;
            });
        }

        private static async Task<T> executeSafeAsync<T>(Func<AuthenticationManagerClient, Task<T>> call)
        {
            var client = new AuthenticationManagerClient();
            try
            {
                T result = await call(client);
                client.Close();
                return result;
            }
            catch (Exception)
            {
                client.Abort();
                throw;
            }
        }

        private async Task<bool> connectSocialServiceAsync(string username)
        {
            try
            {
                await socialService.connectAsync(username);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError(
                    $"Failed to connect social service for {username}: {ex.Message}");
                return false;
            }
        }
    }
}