using MindWeaveClient.Properties.Langs;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace MindWeaveClient.ViewModel.Main
{
    public class LanguageOption
    {
        public string Name { get; set; }
        public string Code { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

    public class SettingsViewModel : BaseViewModel
    {
        private double musicVolumeValue;
        private double soundEffectsVolumeValue;
        private LanguageOption selectedLanguageValue;
        private List<LanguageOption> availableLanguagesValue;

        public double musicVolume
        {
            get => musicVolumeValue;
            set
            {
                musicVolumeValue = value;
                OnPropertyChanged();
                AudioManager.SetMusicVolume(value / 100.0);
            }
        }

        public double soundEffectsVolume
        {
            get => soundEffectsVolumeValue;
            set
            {
                soundEffectsVolumeValue = value;
                OnPropertyChanged();
                AudioManager.SetSoundEffectsVolume(value / 100.0);
            }
        }

        public LanguageOption selectedLanguage
        {
            get => selectedLanguageValue;
            set { selectedLanguageValue = value; OnPropertyChanged(); }
        }

        public List<LanguageOption> availableLanguages
        {
            get => availableLanguagesValue;
            private set { availableLanguagesValue = value; OnPropertyChanged(); }
        }

        public ICommand saveCommand { get; }
        public ICommand cancelCommand { get; }
        public ICommand showCreditsCommand { get; }

        private Window currentWindow;

        public SettingsViewModel(Window window)
        {
            currentWindow = window;
            loadSettings();
            initializeLanguages();

            saveCommand = new RelayCommand(p => executeSave());
            cancelCommand = new RelayCommand(p => executeCancel());
            showCreditsCommand = new RelayCommand(p => executeShowCredits());
        }

        private void initializeLanguages()
        {
            availableLanguages = new List<LanguageOption>
            {
                new LanguageOption { Name = Lang.SettingsOptEnglish, Code = "en-US" },
                new LanguageOption { Name = Lang.SettingsOptSpanish, Code = "es-MX" }
            };

            string currentLangCode = Properties.Settings.Default.languageCode;
            selectedLanguage = availableLanguages.FirstOrDefault(lang => lang.Code == currentLangCode) ?? availableLanguages.First();
        }

        private void loadSettings()
        {
            musicVolume = Properties.Settings.Default.MusicVolumeSetting;
            soundEffectsVolume = Properties.Settings.Default.SoundEffectsVolumeSetting;

            AudioManager.SetMusicVolume(musicVolume / 100.0);
            AudioManager.SetSoundEffectsVolume(soundEffectsVolume / 100.0);
        }

        private void executeSave()
        {
            Properties.Settings.Default.MusicVolumeSetting = musicVolume;
            Properties.Settings.Default.SoundEffectsVolumeSetting = soundEffectsVolume;
            string previousLanguageCode = Properties.Settings.Default.languageCode;
            Properties.Settings.Default.languageCode = selectedLanguage.Code;
            Properties.Settings.Default.Save();
            AudioManager.SetMusicVolume(musicVolume / 100.0);
            AudioManager.SetSoundEffectsVolume(soundEffectsVolume / 100.0);

            if (previousLanguageCode != selectedLanguage.Code)
            {
                applyLanguageChange();
            }
            if (currentWindow != null)
            {
                currentWindow.DialogResult = true;
                currentWindow.Close();
            }
        }

        private void applyLanguageChange()
        {
            MessageBoxResult result = MessageBox.Show(
                "Language changed. The application needs to restart to apply changes. Restart now?",
                "Restart Required",
                MessageBoxButton.YesNo,
                MessageBoxImage.Information);

            if (result == MessageBoxResult.Yes)
            {
                System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                Application.Current.Shutdown();
            }
        }


        private void executeCancel()
        {
            if (currentWindow != null)
            {
                currentWindow.DialogResult = false;
                currentWindow.Close();
            }
        }

        private void executeShowCredits()
        {
            MessageBox.Show("Mind Weave\nDeveloped by:\nAldo Antonio Campos Gómez\nAngel Jonathan Puch Hernández\n\n© 2025", "Credits"); // TODO: Añadir a Lang.resx
        }
    }
}