using MindWeaveClient.AuthenticationService;
using MindWeaveClient.Services.Abstractions;
using System;
using System.Threading.Tasks;

namespace MindWeaveClient.Services.Implementations
{
    public class AuthenticationService : IAuthenticationService
    {
        public async Task<LoginServiceResultDto> loginAsync(string email, string password)
        {
            using (var client = new AuthenticationManagerClient())
            {
                var loginCredentials = new LoginDto
                {
                    email = email,
                    password = password
                };

                LoginResultDto result = await client.loginAsync(loginCredentials);

                if (result.operationResult.success)
                {
                    var (socialConnected, matchmakingConnected) = handleSuccessfulLogin(result.username, result.avatarPath);

                    return new LoginServiceResultDto(result, socialConnected, matchmakingConnected);
                }
                else
                {
                    return new LoginServiceResultDto(result);
                }
            }
        }

        public async Task<OperationResultDto> resendVerificationCodeAsync(string email)
        {
            using (var client = new AuthenticationManagerClient())
            {
                OperationResultDto result = await client.resendVerificationCodeAsync(email);
                return result;
            }
        }

        private (bool socialConnected, bool matchmakingConnected) handleSuccessfulLogin(string username, string avatarPath)
        {
            bool socialConnected;
            bool matchmakingConnected;

            SessionService.SetSession(username, avatarPath);

            try
            {
                socialConnected = SocialServiceClientManager.instance.Connect(username);
            }
            catch (Exception)
            {
                socialConnected = false;
            }

            try
            {
                MatchmakingServiceClientManager.instance.Connect();
                matchmakingConnected = true;
            }
            catch (Exception)
            {
                matchmakingConnected = false;
            }

            return (socialConnected, matchmakingConnected);
        }
    }
}
