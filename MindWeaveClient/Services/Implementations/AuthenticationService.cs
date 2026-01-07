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
        // ELIMINADO: IHeartbeatService

        public AuthenticationService(ISocialService socialService)
        {
            this.socialService = socialService;
            // ELIMINADO: heartbeatService
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

                // Conectar al servicio social (esto establece la Reliable Session)
                bool socialConnected = await connectSocialServiceSafeAsync(result.Username);

                // ELIMINADO: startHeartbeatServiceSafeAsync
                // Las Reliable Sessions manejan la detección de desconexión automáticamente

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
                System.Diagnostics.Debug.WriteLine("[AUTH] Social service connected with Reliable Session");
                return true;
            }
            catch (CommunicationException ex)
            {
                System.Diagnostics.Debug.WriteLine($"[AUTH] Social service connection failed: {ex.Message}");
                return false;
            }
            catch (TimeoutException ex)
            {
                System.Diagnostics.Debug.WriteLine($"[AUTH] Social service connection timeout: {ex.Message}");
                return false;
            }
            catch (SocketException ex)
            {
                System.Diagnostics.Debug.WriteLine($"[AUTH] Social service socket error: {ex.Message}");
                return false;
            }
        }

        // ELIMINADO: startHeartbeatServiceSafeAsync

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