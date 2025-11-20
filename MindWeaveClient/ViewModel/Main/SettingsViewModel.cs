using MindWeaveClient.Properties.Langs;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using MindWeaveClient.Utilities.Abstractions;
using System;

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
        private readonly IAudioService audioService;
        private readonly IDialogService dialogService;

        private double musicVolumeValue;
        private double soundEffectsVolumeValue;
        private LanguageOption selectedLanguageValue;
        private List<LanguageOption> availableLanguagesValue;

        private Action<bool?> setDialogResultAction;
        private Action closeWindowAction;

        public double MusicVolume
        {
            get => musicVolumeValue;
            set
            {
                musicVolumeValue = value;
                OnPropertyChanged();
                audioService.setMusicVolume(value / 100.0);
            }
        }

        public double SoundEffectsVolume
        {
            get => soundEffectsVolumeValue;
            set
            {
                soundEffectsVolumeValue = value;
                OnPropertyChanged();
                audioService.setSoundEffectsVolume(value / 100.0);
            }
        }

        public LanguageOption SelectedLanguage
        {
            get => selectedLanguageValue;
            set
            {
                selectedLanguageValue = value;
                OnPropertyChanged();
            }
        }

        public List<LanguageOption> AvailableLanguages
        {
            get => availableLanguagesValue;
            private set
            {
                availableLanguagesValue = value;
                OnPropertyChanged();
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand ShowCreditsCommand { get; }

        public SettingsViewModel(IAudioService audioService, IDialogService dialogService)
        {
            this.audioService = audioService;
            this.dialogService = dialogService;

            loadSettings();
            initializeLanguages();

            SaveCommand = new RelayCommand(p => executeSave());
            CancelCommand = new RelayCommand(p => executeCancel());
            ShowCreditsCommand = new RelayCommand(p => executeShowCredits());
        }

        public void setCloseAction(Action<bool?> setDialogResultAction, Action closeWindowAction)
        {
            this.setDialogResultAction = setDialogResultAction;
            this.closeWindowAction = closeWindowAction;
        }

        private void initializeLanguages()
        {
            AvailableLanguages = new List<LanguageOption>
            {
                new LanguageOption { Name = Lang.SettingsOptEnglish, Code = "en-US" },
                new LanguageOption { Name = Lang.SettingsOptSpanish, Code = "es-MX" }
            };

            string currentLangCode = Properties.Settings.Default.languageCode;
            SelectedLanguage = AvailableLanguages.FirstOrDefault(lang => lang.Code == currentLangCode) ?? AvailableLanguages[0];
        }

        private void loadSettings()
        {
            musicVolumeValue = Properties.Settings.Default.MusicVolumeSetting;
            soundEffectsVolumeValue = Properties.Settings.Default.SoundEffectsVolumeSetting;

            OnPropertyChanged(nameof(MusicVolume));
            OnPropertyChanged(nameof(SoundEffectsVolume));
        }

        private void executeSave()
        {
            Properties.Settings.Default.MusicVolumeSetting = MusicVolume;
            Properties.Settings.Default.SoundEffectsVolumeSetting = SoundEffectsVolume;
            string previousLanguageCode = Properties.Settings.Default.languageCode;
            Properties.Settings.Default.languageCode = SelectedLanguage.Code;
            Properties.Settings.Default.Save();

            if (previousLanguageCode != SelectedLanguage.Code)
            {
                applyLanguageChange();
            }

            setDialogResultAction?.Invoke(true);
            closeWindowAction?.Invoke();
        }

        private void applyLanguageChange()
        {
            bool restartTime = dialogService.showConfirmation(
                Lang.RestartRequiredMessage,
                Lang.RestartRequiredTitle
            );

            if (restartTime)
            {
                System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                Application.Current.Shutdown();
            }
        }

        private void executeCancel()
        {
            loadSettings();

            audioService.setMusicVolume(MusicVolume / 100.0);
            audioService.setSoundEffectsVolume(SoundEffectsVolume / 100.0);

            setDialogResultAction?.Invoke(false);
            closeWindowAction?.Invoke();
        }

        private void executeShowCredits()
        {
            dialogService.showInfo(
                Lang.CreditsContent,
                Lang.SettingsBtnCredits
            );
        }
    }
}