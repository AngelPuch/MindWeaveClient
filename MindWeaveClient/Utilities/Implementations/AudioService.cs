using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using MindWeaveClient.Properties;
using MindWeaveClient.Utilities.Abstractions;

namespace MindWeaveClient.Utilities.Implementations
{
    internal class AudioService : IAudioService
    {
        private const string MUSIC_RESOURCE_PATH = "/Resources/Audio/audio_background.mp3";
        private const string SFX_RESOURCE_PATH_FORMAT = "/MindWeaveClient;component/Resources/Audio/{0}";

        private const string TEMP_MUSIC_FILE_PREFIX = "MindWeave_";
        private const string TEMP_SFX_FILE_PREFIX = "MW_SFX_";
        private const string MP3_EXTENSION = ".mp3";

        private const double VOLUME_MIN = 0.0;
        private const double VOLUME_MAX = 1.0;
        private const double VOLUME_DEFAULT = 0.5;

        private const double VOLUME_PERCENTAGE_DIVISOR = 100.0;

        private readonly MediaPlayer musicPlayer;
        private readonly MediaPlayer sfxPlayer;
        private bool isMusicLoaded;
        private string tempMusicFilePath;
        private bool isDisposed;

        private bool disposedValue;


        public AudioService()
        {
            musicPlayer = new MediaPlayer();
            sfxPlayer = new MediaPlayer();
            loadInitialVolumes();
        }

        public void initialize()
        {
            try
            {
                initializeBackgroundMusic();
            }
            catch (IOException)
            {
                isMusicLoaded = false;
                cleanupTempFile();
            }
            catch (InvalidOperationException)
            {
                isMusicLoaded = false;
                cleanupTempFile();
            }
            catch (UnauthorizedAccessException)
            {
                isMusicLoaded = false;
                cleanupTempFile();
            }
        }

        public void setMusicVolume(double volume)
        {
            setMusicVolumeInternal(volume);
        }

        public void setSoundEffectsVolume(double volume)
        {
            setSoundEffectsVolumeInternal(volume);
        }

        public void stopMusic()
        {
            try
            {
                if (musicPlayer.CanPause)
                {
                    musicPlayer.Stop();
                }
            }
            catch (InvalidOperationException)
            {
                // ignored
            }
        }

        public void playSoundEffect(string soundFileName)
        {
            if (string.IsNullOrWhiteSpace(soundFileName)) return;

            try
            {
                playSound(soundFileName);
            }
            catch (IOException)
            {
                // ignored
            }
            catch (InvalidOperationException)
            {
                // ignored
            }
            catch (UnauthorizedAccessException)
            {
                // ignored
            }
        }


        private void initializeBackgroundMusic()
        {
            Uri resourceUri = new Uri(MUSIC_RESOURCE_PATH, UriKind.Relative);
            var resourceInfo = Application.GetResourceStream(resourceUri);

            if (resourceInfo == null)
            {
                isMusicLoaded = false;
                return;
            }

            tempMusicFilePath = createTempMusicFile(resourceInfo);
            configureMusicPlayer(tempMusicFilePath);
        }

        private static string createTempMusicFile(System.Windows.Resources.StreamResourceInfo resourceInfo)
        {
            string tempPath = Path.Combine(
                Path.GetTempPath(),
                TEMP_MUSIC_FILE_PREFIX + Guid.NewGuid() + MP3_EXTENSION);

            using (Stream resourceStream = resourceInfo.Stream)
            using (FileStream fileStream = new FileStream(tempPath, FileMode.Create, FileAccess.Write))
            {
                resourceStream.CopyTo(fileStream);
            }

            return tempPath;
        }

        private void configureMusicPlayer(string filePath)
        {
            Uri fileUri = new Uri(filePath, UriKind.Absolute);

            musicPlayer.MediaOpened += onMusicMediaOpened;
            musicPlayer.MediaFailed += onMusicMediaFailed;
            musicPlayer.MediaEnded += onMusicMediaEnded;

            musicPlayer.Open(fileUri);
        }

        private void loadInitialVolumes()
        {
            try
            {
                double initialMusicVolume = Settings.Default.MusicVolumeSetting;
                double initialSfxVolume = Settings.Default.SoundEffectsVolumeSetting;

                setMusicVolumeInternal(initialMusicVolume / VOLUME_PERCENTAGE_DIVISOR);
                setSoundEffectsVolumeInternal(initialSfxVolume / VOLUME_PERCENTAGE_DIVISOR);
            }
            catch (Exception)
            {
                setMusicVolumeInternal(VOLUME_DEFAULT);
                setSoundEffectsVolumeInternal(VOLUME_DEFAULT);
            }
        }

        private void onMusicMediaOpened(object sender, EventArgs e)
        {
            isMusicLoaded = true;

            Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Loaded,
                new Action(() =>
                {
                    if (isMusicLoaded && musicPlayer.Source != null && musicPlayer.Volume > VOLUME_MIN)
                    {
                        musicPlayer.Play();
                    }
                }));
        }

        private void onMusicMediaFailed(object sender, ExceptionEventArgs e)
        {
            isMusicLoaded = false;
            cleanupTempFile();
        }

        private void onMusicMediaEnded(object sender, EventArgs e)
        {
            musicPlayer.Position = TimeSpan.Zero;
            musicPlayer.Play();
        }

        private void setMusicVolumeInternal(double volume)
        {
            musicPlayer.Volume = clampVolume(volume);
        }

        private void setSoundEffectsVolumeInternal(double volume)
        {
            sfxPlayer.Volume = clampVolume(volume);
        }

        private static double clampVolume(double volume)
        {
            if (volume < VOLUME_MIN) return VOLUME_MIN;
            if (volume > VOLUME_MAX) return VOLUME_MAX;
            return volume;
        }

        private void playSound(string soundFileName)
        {
            string resourcePath = string.Format(SFX_RESOURCE_PATH_FORMAT, soundFileName);
            Uri resourceUri = new Uri(resourcePath, UriKind.Relative);
            var resourceInfo = Application.GetResourceStream(resourceUri);

            if (resourceInfo == null)
            {
                return;
            }

            string tempPath = getOrCreateSfxTempFile(soundFileName, resourceInfo);

            sfxPlayer.Open(new Uri(tempPath, UriKind.Absolute));
            sfxPlayer.Play();
        }

        private static string getOrCreateSfxTempFile(string soundFileName, System.Windows.Resources.StreamResourceInfo resourceInfo)
        {
            string tempPath = Path.Combine(Path.GetTempPath(), TEMP_SFX_FILE_PREFIX + soundFileName);

            if (!File.Exists(tempPath))
            {
                using (Stream resourceStream = resourceInfo.Stream)
                using (FileStream fileStream = new FileStream(tempPath, FileMode.Create, FileAccess.Write))
                {
                    resourceStream.CopyTo(fileStream);
                }
            }

            return tempPath;
        }

        private void cleanupTempFile()
        {
            if (string.IsNullOrEmpty(tempMusicFilePath)) return;

            try
            {
                if (File.Exists(tempMusicFilePath))
                {
                    File.Delete(tempMusicFilePath);
                }
            }
            catch (IOException)
            {
                // ignored
            }
            catch (UnauthorizedAccessException )
            {
                // ignored
            }
            finally
            {
                tempMusicFilePath = null;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }


        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed) return;

            if (disposing)
            {
                try
                {
                    stopMusic();

                    musicPlayer.MediaOpened -= onMusicMediaOpened;
                    musicPlayer.MediaFailed -= onMusicMediaFailed;
                    musicPlayer.MediaEnded -= onMusicMediaEnded;

                    musicPlayer.Close();
                    sfxPlayer.Close();
                }
                catch (InvalidOperationException)
                {
                    // ignored
                }
                finally
                {
                    cleanupTempFile();
                }
            }

            isDisposed = true;
        }

    }
}