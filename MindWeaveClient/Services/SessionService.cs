using System;

namespace MindWeaveClient.Services
{
    public static class SessionService
    {
        public static string Username { get; private set; }
        public static string AvatarPath { get; private set; }
        public static bool IsGuest { get; private set; }
        public static string PendingVerificationEmail { get; set; }
        public static event EventHandler AvatarPathChanged;

        public static void SetSession(string username, string avatarPath, bool isGuest = false)
        {
            SessionService.Username = username;
            SessionService.AvatarPath = avatarPath ?? "/Resources/Images/Avatar/default_avatar.png";
            SessionService.IsGuest = isGuest;
        }

        public static void UpdateAvatarPath(string newAvatarPath)
        {
            if (!IsGuest)
            {
                AvatarPath = newAvatarPath;
                AvatarPathChanged?.Invoke(null, EventArgs.Empty);
            }
        }

        public static void ClearSession()
        {
            Username = null;
            AvatarPath = null;
            IsGuest = false;
            PendingVerificationEmail = null;
        }
    }
}