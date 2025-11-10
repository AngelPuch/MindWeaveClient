using MindWeaveClient.AuthenticationService;

namespace MindWeaveClient.Services
{
    public class LoginServiceResultDto
    {
        public LoginResultDto WcfLoginResult { get; }
        public bool IsSocialServiceConnected { get; }
        public bool IsMatchmakingServiceConnected { get; }

        public LoginServiceResultDto(LoginResultDto wcfLoginResult, bool socialConnected, bool matchmakingConnected)
        {
            this.WcfLoginResult = wcfLoginResult;
            this.IsSocialServiceConnected = socialConnected;
            this.IsMatchmakingServiceConnected = matchmakingConnected;
        }

        public LoginServiceResultDto(LoginResultDto wcfLoginResult)
        {
            this.WcfLoginResult = wcfLoginResult;
            this.IsSocialServiceConnected = false;
            this.IsMatchmakingServiceConnected = false;
        }
    }
}
