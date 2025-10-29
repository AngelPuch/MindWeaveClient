namespace MindWeaveClient.Services
{
    public static class SessionService
    {
        public static string username { get; private set; }
        public static string avatarPath { get; private set; }
        public static bool isGuest { get; private set; }

        public static void setSession(string username, string avatarPath, bool isGuest = false)
        {
            SessionService.username = username;
            SessionService.avatarPath = avatarPath ?? "/Resources/Images/Avatar/default_avatar.png";
            SessionService.isGuest = isGuest;
        }

        public static void updateAvatarPath(string newAvatarPath)
        {
            if (!isGuest)
            {
                avatarPath = newAvatarPath;
            }
        }

        public static void clearSession()
        {
            username = null;
            avatarPath = null;
            isGuest = false;
        }
    }
}