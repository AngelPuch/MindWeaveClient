using MindWeaveClient.AuthenticationService;

namespace MindWeaveClient.Services
{
    public class LoginServiceResultDto
    {
        public LoginResultDto wcfLoginResult { get; }
        public bool isSocialServiceConnected { get; }
        public bool isMatchmakingServiceConnected { get; }

        public LoginServiceResultDto(LoginResultDto wcfLoginResult, bool socialConnected, bool matchmakingConnected)
        {
            this.wcfLoginResult = wcfLoginResult;
            this.isSocialServiceConnected = socialConnected;
            this.isMatchmakingServiceConnected = matchmakingConnected;
        }

        public LoginServiceResultDto(LoginResultDto wcfLoginResult)
        {
            this.wcfLoginResult = wcfLoginResult;
            this.isSocialServiceConnected = false;
            this.isMatchmakingServiceConnected = false;
        }
    }
}
