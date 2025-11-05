using System.Threading.Tasks;
using MindWeaveClient.AuthenticationService;

namespace MindWeaveClient.Services.Abstractions
{
    public interface IAuthenticationService
    {
        Task<LoginServiceResultDto> loginAsync(string email, string password);
        Task<OperationResultDto> resendVerificationCodeAsync(string email);
        Task<OperationResultDto> registerAsync(UserProfileDto profile, string password);
        Task<OperationResultDto> verifyAccountAsync(string email, string code);
        Task<OperationResultDto> sendPasswordRecoveryCodeAsync(string email);
        Task<OperationResultDto> resetPasswordWithCodeAsync(string email, string code, string newPassword);
    }
}