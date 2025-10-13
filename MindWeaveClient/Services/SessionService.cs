namespace MindWeaveClient.Services
{
    public static class SessionService
    {
        public static string username { get; private set; }
        public static string avatarPath { get; private set; }

        public static void setSession(string username, string avatarPath)
        {
            SessionService.username = username;
            SessionService.avatarPath = avatarPath;
        }

        public static void clearSession()
        {
            username = null;
            avatarPath = null;
        }
    }
}