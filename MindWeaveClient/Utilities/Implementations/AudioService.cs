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
        private readonly MediaPlayer musicPlayer = new MediaPlayer();
        private readonly MediaPlayer sfxPlayer = new MediaPlayer();
        private bool isMusicLoaded;
        private string tempMusicFilePath;

        
        private bool disposedValue;

        public AudioService()
        {
            loadInitialVolumes();
        }

        public void initialize()
        {
            try
            {
                string resourcePath = "/Resources/Audio/audio_background.mp3";
                Uri resourceUri = new Uri(resourcePath, UriKind.Relative);
                var resourceInfo = Application.GetResourceStream(resourceUri);

                if (resourceInfo != null)
                {
                    tempMusicFilePath = Path.Combine(Path.GetTempPath(), $"MindWeave_{Guid.NewGuid()}.mp3");
                    using (Stream resourceStream = resourceInfo.Stream)
                    using (FileStream fileStream = new FileStream(tempMusicFilePath, FileMode.Create, FileAccess.Write))
                    {
                        resourceStream.CopyTo(fileStream);
                    }

                    Uri fileUri = new Uri(tempMusicFilePath, UriKind.Absolute);

                    musicPlayer.MediaOpened += (sender, e) =>
                    {
                        isMusicLoaded = true;

                        Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() =>
                        {
                            if (isMusicLoaded && musicPlayer.Source != null && musicPlayer.Volume > 0)
                            {
                                musicPlayer.Play();
                            }
                        }));
                    };

                    musicPlayer.MediaFailed += (sender, e) =>
                    {
                        isMusicLoaded = false;
                        cleanupTempFile();
                    };

                    musicPlayer.MediaEnded += (s, e) =>
                    {
                        musicPlayer.Position = TimeSpan.Zero;
                        musicPlayer.Play();
                    };
                    musicPlayer.Open(fileUri);
                }
                else
                {
                    isMusicLoaded = false;
                }
            }
            catch (Exception ex)
            {
                isMusicLoaded = false;
                cleanupTempFile();
            }
        }

        private void cleanupTempFile()
        {
            if (tempMusicFilePath != null && File.Exists(tempMusicFilePath))
            {
                try
                {
                    File.Delete(tempMusicFilePath);
                }
                catch
                {
                    // Ignoramos errores de borrado al cerrar para no bloquear el dispose
                }
                tempMusicFilePath = null;
            }
        }

        private void loadInitialVolumes()
        {
            try
            {
                double initialMusicVolume = Settings.Default.MusicVolumeSetting;
                double initialSfxVolume = Settings.Default.SoundEffectsVolumeSetting;
                setMusicVolumeInternal(initialMusicVolume / 100.0);
                setSoundEffectsVolumeInternal(initialSfxVolume / 100.0);
            }
            catch (Exception ex)
            {
                setMusicVolumeInternal(0.5);
                setSoundEffectsVolumeInternal(0.5);
            }
        }

        private void setMusicVolumeInternal(double volume)
        {
            if (volume < 0) volume = 0;
            if (volume > 1) volume = 1;
            musicPlayer.Volume = volume;
        }

        private void setSoundEffectsVolumeInternal(double volume)
        {
            if (volume < 0) volume = 0;
            if (volume > 1) volume = 1;
            sfxPlayer.Volume = volume;
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
            if (musicPlayer.CanPause)
            {
                musicPlayer.Stop();
            }
        }

        public void playSoundEffect(string soundFileName)
        {
            if (string.IsNullOrWhiteSpace(soundFileName)) return;

            try
            {
                Uri resourceUri = new Uri($"/MindWeaveClient;component/Resources/Audio/{soundFileName}", UriKind.Relative);
                var resourceInfo = Application.GetResourceStream(resourceUri);

                if (resourceInfo != null)
                {
                    string tempPath = Path.Combine(Path.GetTempPath(), $"MW_SFX_{soundFileName}");

                    if (!File.Exists(tempPath))
                    {
                        using (Stream resourceStream = resourceInfo.Stream)
                        using (FileStream fileStream = new FileStream(tempPath, FileMode.Create, FileAccess.Write))
                        {
                            resourceStream.CopyTo(fileStream);
                        }
                    }

                    sfxPlayer.Open(new Uri(tempPath, UriKind.Absolute));
                    sfxPlayer.Play();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error playing SFX: {ex.Message}");
            }
        }


        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    stopMusic();
                    musicPlayer.Close();
                    sfxPlayer.Close();
                    cleanupTempFile();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}