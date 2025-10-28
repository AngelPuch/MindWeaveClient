using System;
using System.Windows.Media;
using MindWeaveClient.Properties;// Necesario para MediaPlayer y Uri

public static class AudioManager
{
    private static MediaPlayer musicPlayer = new MediaPlayer();
    private static MediaPlayer sfxPlayer = new MediaPlayer(); // Para efectos

    static AudioManager()
    {
        // Carga la música de fondo al iniciar
        // La ruta empieza con '/' y usa el nombre del ensamblado (tu proyecto)
        // seguido de ';component/' y la ruta relativa dentro del proyecto.
        try
        {
            // OJO: Reemplaza "MindWeaveClient" si el nombre de tu ensamblado es diferente
            Uri musicUri = new Uri("pack://application:,,,/MindWeaveClient;component/Resources/Audio/audio_background.mp3", UriKind.Absolute);
            musicPlayer.Open(musicUri);
            musicPlayer.MediaEnded += (s, e) => { musicPlayer.Position = TimeSpan.Zero; musicPlayer.Play(); }; // Para que se repita (loop)
            Console.WriteLine("Music loaded successfully."); // Log
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading background music: {ex.Message}");
            // Considera mostrar un MessageBox o loggear el error
        }

        // Carga los ajustes de volumen iniciales
        LoadInitialVolumes();
    }

    private static void LoadInitialVolumes()
    {
        // Lee los valores guardados (asegúrate que existan en Settings.settings)
        double initialMusicVolume = Settings.Default.MusicVolumeSetting;
        double initialSfxVolume = Settings.Default.SoundEffectsVolumeSetting;

        // Aplica los volúmenes (convirtiendo 0-100 a 0.0-1.0)
        SetMusicVolume(initialMusicVolume / 100.0);
        SetSoundEffectsVolume(initialSfxVolume / 100.0);
        Console.WriteLine($"Initial volumes loaded: Music={musicPlayer.Volume}, SFX={sfxPlayer.Volume}");
    }


    // --- Métodos para controlar volumen (como los tenías en el ViewModel) ---
    public static void SetMusicVolume(double volume) // volume de 0.0 a 1.0
    {
        if (volume < 0) volume = 0;
        if (volume > 1) volume = 1;
        musicPlayer.Volume = volume;
        Console.WriteLine($"Music volume set to: {musicPlayer.Volume}");
    }

    public static void SetSoundEffectsVolume(double volume) // volume de 0.0 a 1.0
    {
        if (volume < 0) volume = 0;
        if (volume > 1) volume = 1;
        // Tendrías que aplicar esto a todos los MediaPlayers de SFX si usas varios,
        // o tener un volumen base global para efectos.
        sfxPlayer.Volume = volume; // Ejemplo simple
        Console.WriteLine($"SFX volume set to: {sfxPlayer.Volume}");
    }

    // --- Métodos para reproducir ---
    public static void PlayMusic()
    {
        try
        {
            if (musicPlayer.Source != null)
            {
                musicPlayer.Play();
                Console.WriteLine("Music playing.");
            }
            else
            {
                Console.WriteLine("Cannot play music, source is null.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error playing music: {ex.Message}");
        }
    }

    public static void StopMusic()
    {
        try
        {
            if (musicPlayer.CanPause)
            {
                musicPlayer.Stop(); // O Pause() si quieres poder reanudar
                Console.WriteLine("Music stopped.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error stopping music: {ex.Message}");
        }
    }


    public static void PlaySoundEffect(string soundFileName) // Pasas solo el nombre del archivo, ej: "piece_placed.wav"
    {
        try
        {
            // Construye la Pack URI completa
            Uri sfxUri = new Uri($"pack://application:,,,/MindWeaveClient;component/Resources/Audio/{soundFileName}", UriKind.Absolute);

            // Podrías usar un MediaPlayer diferente para cada sonido si se solapan,
            // o reutilizar uno si solo suena uno a la vez. Reutilizar es más simple aquí:
            sfxPlayer.Open(sfxUri);
            sfxPlayer.Play();
            Console.WriteLine($"Playing SFX: {soundFileName}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error playing sound effect '{soundFileName}': {ex.Message}");
        }
    }

    // Puedes añadir más métodos como PauseMusic, etc.
}