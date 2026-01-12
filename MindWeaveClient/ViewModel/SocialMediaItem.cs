namespace MindWeaveClient.ViewModel
{
    public class SocialMediaItem : BaseViewModel
    {
        private string username;

        public int IdSocialMediaPlatform { get; set; }
        public string PlatformName { get; set; }

        public string Username
        {
            get { return this.username; }
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
                    return "/Resources/Images/Social/default.png";
                }

                switch (PlatformName.ToLower())
                {
                    case "facebook":
                        return "/Resources/Images/Social/facebook.png";
                    case "instagram":
                        return "/Resources/Images/Social/instagram.png";
                    case "youtube":
                        return "/Resources/Images/Social/youtube.png";
                    default:
                        return "/Resources/Images/Social/default.png";
                }
            }
        }
    }
}
