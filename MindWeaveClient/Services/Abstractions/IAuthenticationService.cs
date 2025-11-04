using System.Threading.Tasks;
using MindWeaveClient.AuthenticationService;

namespace MindWeaveClient.Services.Abstractions
{
    public interface IAuthenticationService
    {
        Task<LoginServiceResultDto> loginAsync(string email, string password);
        Task<OperationResultDto> resendVerificationCodeAsync(string email);
    }
}