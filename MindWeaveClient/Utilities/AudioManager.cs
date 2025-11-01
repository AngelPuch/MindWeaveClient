using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using MindWeaveClient.Properties;

namespace MindWeaveClient.Utilities
{
    internal static class AudioManager
    {
        private static MediaPlayer musicPlayer = new MediaPlayer();
        private static MediaPlayer sfxPlayer = new MediaPlayer();
        private static bool isMusicLoaded;
        private static string tempMusicFilePath;

        public static void Initialize()
        {
        }

        static AudioManager()
        {
            loadInitialVolumes();

            try
            {
                string resourcePath = "/Resources/Audio/audio_background.mp3";
                Uri resourceUri = new Uri(resourcePath, UriKind.Relative);
                var resourceInfo = Application.GetResourceStream(resourceUri);

                if (resourceInfo != null)
                {
                    // Crear archivo temporal
                    tempMusicFilePath = Path.Combine(Path.GetTempPath(), $"MindWeave_{Guid.NewGuid()}.mp3");
                    Debug.WriteLine($"AudioManager: Creating temporary music file at: {tempMusicFilePath}");
                    using (Stream resourceStream = resourceInfo.Stream)
                    using (FileStream fileStream = new FileStream(tempMusicFilePath, FileMode.Create, FileAccess.Write))
                    {
                        resourceStream.CopyTo(fileStream);
                    }
                    Debug.WriteLine($"AudioManager: Successfully copied resource stream to temporary file.");

                    Uri fileUri = new Uri(tempMusicFilePath, UriKind.Absolute);

                    // --- Evento MediaOpened Simplificado ---
                    musicPlayer.MediaOpened += (sender, e) =>
                    {
                        Debug.WriteLine($"!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                        Debug.WriteLine($"!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                        isMusicLoaded = true;
                        Debug.WriteLine($"---> AudioManager: MediaOpened event FIRED for TEMP FILE: {tempMusicFilePath}");
                        Debug.WriteLine($"---> AudioManager: Volume JUST BEFORE Dispatcher: {musicPlayer.Volume}");

                        // *** Intento de Play con Retraso usando Dispatcher ***
                        Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() => // Cambiado a Loaded por si acaso
                        {
                            Debug.WriteLine($"---> AudioManager: Dispatcher action STARTED."); // Log para ver si entra aquí
                            try
                            {
                                if (isMusicLoaded && musicPlayer.Source != null && musicPlayer.Volume > 0) // Triple chequeo
                                {
                                    Debug.WriteLine($"---> AudioManager: Volume INSIDE Dispatcher BEFORE Play: {musicPlayer.Volume}"); // Re-verifica volumen
                                    musicPlayer.Play();
                                    Debug.WriteLine($"---> AudioManager: musicPlayer.Play() CALLED via Dispatcher."); // Log *después* de llamar Play
                                    // Añade un pequeño retraso y verifica si está sonando
                                    DispatcherTimer checkTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
                                    checkTimer.Tick += (s, args) => {
                                        checkTimer.Stop();
                                        Debug.WriteLine($"---> AudioManager: Play check after 500ms - HasDuration: {musicPlayer.HasAudio}, Position: {musicPlayer.Position}");
                                    };
                                    checkTimer.Start();
                                }
                                else
                                {
                                    Debug.WriteLine($"---> AudioManager: Play() SKIPPED in Dispatcher. isMusicLoaded={isMusicLoaded}, SourceNull={musicPlayer.Source == null}, Volume={musicPlayer.Volume}");
                                }
                            }
                            catch (Exception playEx)
                            {
                                Debug.WriteLine($"---> AudioManager ERROR: Exception calling Play() via Dispatcher - {playEx}");
                            }
                            Debug.WriteLine($"---> AudioManager: Dispatcher action FINISHED.");
                            Debug.WriteLine($"!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                            Debug.WriteLine($"!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                        }));
                    };

                    musicPlayer.MediaFailed += (sender, e) =>
                    {
                        isMusicLoaded = false;
                        // *** Log COMPLETO de la excepción ***
                        Debug.WriteLine($"!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                        Debug.WriteLine($"AudioManager ERROR: MediaFailed event fired for TEMP FILE: {tempMusicFilePath}");
                        if (e.ErrorException != null)
                        {
                            Debug.WriteLine($"   Exception Type: {e.ErrorException.GetType().FullName}");
                            Debug.WriteLine($"   Exception Message: {e.ErrorException.Message}");
                            Debug.WriteLine($"   Stack Trace: {e.ErrorException.StackTrace}");
                            if (e.ErrorException.InnerException != null)
                            {
                                Debug.WriteLine($"   Inner Exception: {e.ErrorException.InnerException}");
                            }
                        }
                        else
                        {
                            Debug.WriteLine($"   No specific ErrorException provided.");
                        }
                        Debug.WriteLine($"!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                        cleanupTempFile();
                    };

                    // Evento MediaEnded (igual que antes)
                    musicPlayer.MediaEnded += (s, e) =>
                    {
                        musicPlayer.Position = TimeSpan.Zero;
                        musicPlayer.Play();
                        Debug.WriteLine("AudioManager: Background music loop.");
                    };


                    Debug.WriteLine($"AudioManager: Calling musicPlayer.Open() with TEMP FILE URI: {fileUri.AbsoluteUri}");
                    musicPlayer.Open(fileUri);

                }
                else
                {
                    Debug.WriteLine($"AudioManager ERROR: Application.GetResourceStream returned null for '{resourcePath}'. Check Build Action and Path.");
                    isMusicLoaded = false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"AudioManager ERROR: Exception during music initialization (Temp File method) - {ex}");
                isMusicLoaded = false;
                cleanupTempFile();
            }

            Application.Current.Exit += OnApplicationExit;
        }

        // --- El resto de métodos sin cambios ---
        private static void cleanupTempFile()
        {
            if (tempMusicFilePath != null && File.Exists(tempMusicFilePath))
            {
                try
                {
                    File.Delete(tempMusicFilePath);
                    Debug.WriteLine($"AudioManager: Deleted temporary music file: {tempMusicFilePath}");
                    tempMusicFilePath = null;
                }
                catch (IOException ex) { Debug.WriteLine($"AudioManager WARNING: Could not delete temporary music file '{tempMusicFilePath}'. Error: {ex.Message}"); }
                catch (UnauthorizedAccessException ex) { Debug.WriteLine($"AudioManager WARNING: No permission to delete temporary music file '{tempMusicFilePath}'. Error: {ex.Message}"); }
            }
        }
        private static void OnApplicationExit(object sender, ExitEventArgs e)
        {
            stopMusic();
            musicPlayer.Close();
            cleanupTempFile();
            Application.Current.Exit -= OnApplicationExit;
        }
        private static void loadInitialVolumes()
        {
            try
            {
                double initialMusicVolume = Settings.Default.MusicVolumeSetting;
                double initialSfxVolume = Settings.Default.SoundEffectsVolumeSetting;
                setMusicVolumeInternal(initialMusicVolume / 100.0);
                setSoundEffectsVolumeInternal(initialSfxVolume / 100.0);
                Debug.WriteLine($"AudioManager: Initial volumes set from settings - Music={musicPlayer.Volume}, SFX={sfxPlayer.Volume}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"AudioManager ERROR: Failed to load initial volumes from settings - {ex.Message}");
                setMusicVolumeInternal(0.5);
                setSoundEffectsVolumeInternal(0.5);
            }
        }
        private static void setMusicVolumeInternal(double volume) { if (volume < 0) volume = 0; if (volume > 1) volume = 1; musicPlayer.Volume = volume; }
        private static void setSoundEffectsVolumeInternal(double volume) { if (volume < 0) volume = 0; if (volume > 1) volume = 1; sfxPlayer.Volume = volume; }
        public static void setMusicVolume(double volume) { setMusicVolumeInternal(volume); Debug.WriteLine($"AudioManager: Music volume set to: {musicPlayer.Volume}"); }
        public static void setSoundEffectsVolume(double volume) { setSoundEffectsVolumeInternal(volume); Debug.WriteLine($"AudioManager: SFX volume set to: {sfxPlayer.Volume}"); }
        public static void playMusic() { if (isMusicLoaded && tempMusicFilePath != null && File.Exists(tempMusicFilePath)) { try { musicPlayer.Play(); Debug.WriteLine("AudioManager: PlayMusic() called manually."); } catch (Exception ex) { Debug.WriteLine($"AudioManager ERROR: Manual PlayMusic() - {ex.Message}"); } } else { Debug.WriteLine($"AudioManager: Manual PlayMusic() called but not ready."); } }
        public static void stopMusic() { try { if (musicPlayer.CanPause) { musicPlayer.Stop(); Debug.WriteLine("AudioManager: StopMusic() called."); } else { Debug.WriteLine($"AudioManager: StopMusic() called but cannot stop."); } } catch (Exception ex) { Debug.WriteLine($"AudioManager ERROR: StopMusic() - {ex.Message}"); } }

        // NOTA: PlaySoundEffect sigue usando la Pack URI. Si también falla, necesitará el mismo
        // tratamiento de archivo temporal que la música de fondo.
        public static void playSoundEffect(string soundFileName)
        {
            if (string.IsNullOrWhiteSpace(soundFileName)) { Debug.WriteLine("AudioManager ERROR: PlaySoundEffect empty filename."); return; }
            try
            {
                Uri sfxUri = new Uri($"pack://application:,,,/MindWeaveClient;component/Resources/Audio/{soundFileName}", UriKind.Absolute);
                sfxPlayer.Open(sfxUri);
                sfxPlayer.Play();
                Debug.WriteLine($"AudioManager: Playing SFX '{soundFileName}'.");
            }
            catch (Exception ex) { Debug.WriteLine($"AudioManager ERROR: Playing SFX '{soundFileName}' - {ex.Message}"); }
        }
    }
} // Fin clase