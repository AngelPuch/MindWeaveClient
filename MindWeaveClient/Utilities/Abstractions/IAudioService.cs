using System;

namespace MindWeaveClient.Utilities.Abstractions
{
    public interface IAudioService : IDisposable
    {
        void initialize();

        void setMusicVolume(double volume);

        void setSoundEffectsVolume(double volume);

        void stopMusic();

        void playSoundEffect(string soundFileName);
    }
}