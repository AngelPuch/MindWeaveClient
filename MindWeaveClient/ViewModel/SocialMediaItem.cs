namespace MindWeaveClient.ViewModel
{
    public class SocialMediaItem : BaseViewModel
    {
        private const string PLATFORM_FACEBOOK = "facebook";
        private const string PLATFORM_INSTAGRAM = "instagram";
        private const string PLATFORM_YOUTUBE = "youtube";

        private const string ICON_PATH_DEFAULT = "/Resources/Images/Social/default.png";
        private const string ICON_PATH_FACEBOOK = "/Resources/Images/Social/facebook.png";
        private const string ICON_PATH_INSTAGRAM = "/Resources/Images/Social/instagram.png";
        private const string ICON_PATH_YOUTUBE = "/Resources/Images/Social/youtube.png";

        private string username;

        public int IdSocialMediaPlatform { get; set; }
        public string PlatformName { get; set; }

        public string Username
        {
            get => this.username;
            set
            {
                this.username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        public string IconPath
        {
            get
            {
                if (string.IsNullOrEmpty(PlatformName))
                {
                    return ICON_PATH_DEFAULT;
                }

                switch (PlatformName.ToLower())
                {
                    case PLATFORM_FACEBOOK:
                        return ICON_PATH_FACEBOOK;
                    case PLATFORM_INSTAGRAM:
                        return ICON_PATH_INSTAGRAM;
                    case PLATFORM_YOUTUBE:
                        return ICON_PATH_YOUTUBE;
                    default:
                        return ICON_PATH_DEFAULT;
                }
            }
        }
    }
}