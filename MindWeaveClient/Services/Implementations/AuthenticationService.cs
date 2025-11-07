using MindWeaveClient.AuthenticationService;
using MindWeaveClient.Services.Abstractions;
using System;
using System.Threading.Tasks;

namespace MindWeaveClient.Services.Implementations
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IMatchmakingService matchmakingService;
        private readonly ISocialService socialService;

        public AuthenticationService(
            IMatchmakingService matchmakingService,
            ISocialService socialService)
        {
            this.matchmakingService = matchmakingService;
            this.socialService = socialService;
        }

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
                bool socialConnected = false;
                bool matchmakingConnected = false;

                if (result.operationResult.success)
                {
                    SessionService.SetSession(result.username, result.avatarPath);

                    socialConnected = this.socialService.connect(result.username);
                    matchmakingConnected = this.matchmakingService.connect();

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

        public async Task<OperationResultDto> registerAsync(UserProfileDto profile, string password)
        {
            using (var client = new AuthenticationManagerClient())
            {
                return await client.registerAsync(profile, password);
            }
        }

        public async Task<OperationResultDto> verifyAccountAsync(string email, string code)
        {
            using (var client = new AuthenticationManagerClient())
            {
                return await client.verifyAccountAsync(email, code);
            }
        }

        public async Task<OperationResultDto> sendPasswordRecoveryCodeAsync(string email)
        {
            using (var client = new AuthenticationManagerClient())
            {
                return await client.sendPasswordRecoveryCodeAsync(email);
            }
        }

        public async Task<OperationResultDto> resetPasswordWithCodeAsync(string email, string code, string newPassword)
        {
            using (var client = new AuthenticationManagerClient())
            {
                return await client.resetPasswordWithCodeAsync(email, code, newPassword);
            }
        }
    }
}
