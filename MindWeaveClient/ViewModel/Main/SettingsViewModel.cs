using MindWeaveClient.Properties.Langs;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using MindWeaveClient.Utilities;

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

        public double MusicVolume
        {
            get => musicVolumeValue;
            set
            {
                musicVolumeValue = value;
                OnPropertyChanged();
                AudioManager.setMusicVolume(value / 100.0);
            }
        }

        public double SoundEffectsVolume
        {
            get => soundEffectsVolumeValue;
            set
            {
                soundEffectsVolumeValue = value;
                OnPropertyChanged();
                AudioManager.setSoundEffectsVolume(value / 100.0);
            }
        }

        public LanguageOption SelectedLanguage
        {
            get => selectedLanguageValue;
            set { selectedLanguageValue = value; OnPropertyChanged(); }
        }

        public List<LanguageOption> AvailableLanguages
        {
            get => availableLanguagesValue;
            private set { availableLanguagesValue = value; OnPropertyChanged(); }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand ShowCreditsCommand { get; }

        private Window currentWindow;

        public SettingsViewModel(Window window)
        {
            currentWindow = window;
            loadSettings();
            initializeLanguages();

            SaveCommand = new RelayCommand(p => executeSave());
            CancelCommand = new RelayCommand(p => executeCancel());
            ShowCreditsCommand = new RelayCommand(p => executeShowCredits());
        }

        private void initializeLanguages()
        {
            AvailableLanguages = new List<LanguageOption>
            {
                new LanguageOption { Name = Lang.SettingsOptEnglish, Code = "en-US" },
                new LanguageOption { Name = Lang.SettingsOptSpanish, Code = "es-MX" }
            };

            string currentLangCode = Properties.Settings.Default.languageCode;
            SelectedLanguage = AvailableLanguages.FirstOrDefault(lang => lang.Code == currentLangCode) ?? AvailableLanguages.First();
        }

        private void loadSettings()
        {
            MusicVolume = Properties.Settings.Default.MusicVolumeSetting;
            SoundEffectsVolume = Properties.Settings.Default.SoundEffectsVolumeSetting;

            AudioManager.setMusicVolume(MusicVolume / 100.0);
            AudioManager.setSoundEffectsVolume(SoundEffectsVolume / 100.0);
        }

        private void executeSave()
        {
            Properties.Settings.Default.MusicVolumeSetting = MusicVolume;
            Properties.Settings.Default.SoundEffectsVolumeSetting = SoundEffectsVolume;
            string previousLanguageCode = Properties.Settings.Default.languageCode;
            Properties.Settings.Default.languageCode = SelectedLanguage.Code;
            Properties.Settings.Default.Save();
            AudioManager.setMusicVolume(MusicVolume / 100.0);
            AudioManager.setSoundEffectsVolume(SoundEffectsVolume / 100.0);

            if (previousLanguageCode != SelectedLanguage.Code)
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