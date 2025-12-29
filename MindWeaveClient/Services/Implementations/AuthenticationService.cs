using MindWeaveClient.AuthenticationService;
using MindWeaveClient.Services.Abstractions;
using System;
using System.Net.Sockets;
using System.ServiceModel;
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

            LoginResultDto result = await executeServiceCallAsync(async (client) =>
                await client.loginAsync(loginCredentials));

            if (result.OperationResult.Success)
            {
                SessionService.setSession(result.PlayerId, result.Username, result.AvatarPath);
                bool socialConnected = await connectSocialServiceSafeAsync(result.Username);

                return new LoginServiceResultDto(result, socialConnected, true);
            }
            else
            {
                return new LoginServiceResultDto(result);
            }
        }

        public async Task<OperationResultDto> resendVerificationCodeAsync(string email)
        {
            return await executeServiceCallAsync(async (client) =>
                await client.resendVerificationCodeAsync(email));
        }

        public async Task<OperationResultDto> registerAsync(UserProfileDto profile, string password)
        {
            return await executeServiceCallAsync(async (client) =>
                await client.registerAsync(profile, password));
        }

        public async Task<OperationResultDto> verifyAccountAsync(string email, string code)
        {
            return await executeServiceCallAsync(async (client) =>
                await client.verifyAccountAsync(email, code));
        }

        public async Task<OperationResultDto> sendPasswordRecoveryCodeAsync(string email)
        {
            return await executeServiceCallAsync(async (client) =>
                await client.sendPasswordRecoveryCodeAsync(email));
        }

        public async Task<OperationResultDto> resetPasswordWithCodeAsync(string email, string code, string newPassword)
        {
            return await executeServiceCallAsync(async (client) =>
                await client.resetPasswordWithCodeAsync(email, code, newPassword));
        }

        public async Task logoutAsync(string username)
        {
            await executeServiceCallAsync(async (client) =>
            {
                await client.logOutAsync(username);
                return 0;
            });
        }

        private static async Task<T> executeServiceCallAsync<T>(Func<AuthenticationManagerClient, Task<T>> serviceCall)
        {
            var client = new AuthenticationManagerClient();

            try
            {
                T result = await serviceCall(client);
                closeClientSafe(client);
                return result;
            }
            catch (CommunicationException)
            {
                abortClientSafe(client);
                throw;
            }
            catch (TimeoutException)
            {
                abortClientSafe(client);
                throw;
            }
            catch (SocketException)
            {
                abortClientSafe(client);
                throw;
            }
        }

        private async Task<bool> connectSocialServiceSafeAsync(string username)
        {
            try
            {
                await socialService.connectAsync(username);
                return true;
            }
            catch (CommunicationException)
            {
                return false;
            }
            catch (TimeoutException)
            {
                return false;
            }
            catch (SocketException)
            {
                return false;
            }
        }

        private static void closeClientSafe(AuthenticationManagerClient client)
        {
            try
            {
                if (client.State == CommunicationState.Opened)
                {
                    client.Close();
                }
            }
            catch (CommunicationException)
            {
                client.Abort();
            }
            catch (TimeoutException)
            {
                client.Abort();
            }
        }

        private static void abortClientSafe(AuthenticationManagerClient client)
        {
            try
            {
                client.Abort();
            }
            catch
            {
                // Ignore
            }
        }


    }
}