using MindWeaveClient.Properties.Langs;
using MindWeaveClient.Utilities.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private const double VOLUME_PERCENTAGE_DIVISOR = 100.0;

        private const string LANGUAGE_CODE_ENGLISH = "en-US";
        private const string LANGUAGE_CODE_SPANISH = "es-MX";

        private const int DEFAULT_LANGUAGE_INDEX = 0;

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
                audioService.setMusicVolume(value / VOLUME_PERCENTAGE_DIVISOR);
            }
        }

        public double SoundEffectsVolume
        {
            get => soundEffectsVolumeValue;
            set
            {
                soundEffectsVolumeValue = value;
                OnPropertyChanged();
                audioService.setSoundEffectsVolume(value / VOLUME_PERCENTAGE_DIVISOR);
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
            this.audioService = audioService ?? throw new ArgumentNullException(nameof(audioService));
            this.dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

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
                new LanguageOption { Name = Lang.SettingsOptEnglish, Code = LANGUAGE_CODE_ENGLISH },
                new LanguageOption { Name = Lang.SettingsOptSpanish, Code = LANGUAGE_CODE_SPANISH }
            };

            string currentLangCode = Properties.Settings.Default.languageCode;
            SelectedLanguage = AvailableLanguages.FirstOrDefault(lang => lang.Code == currentLangCode)
                               ?? AvailableLanguages[DEFAULT_LANGUAGE_INDEX];
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
            saveVolumeSettings();
            bool languageChanged = saveLanguageSetting();

            if (languageChanged)
            {
                promptForRestart();
            }

            closeDialog(dialogResult: true);
        }

        private void executeCancel()
        {
            loadSettings();
            restoreAudioVolumes();
            closeDialog(dialogResult: false);
        }

        private void executeShowCredits()
        {
            dialogService.showInfo(Lang.CreditsContent, Lang.SettingsBtnCredits);
        }

        private void saveVolumeSettings()
        {
            Properties.Settings.Default.MusicVolumeSetting = MusicVolume;
            Properties.Settings.Default.SoundEffectsVolumeSetting = SoundEffectsVolume;
            Properties.Settings.Default.Save();
        }

        private bool saveLanguageSetting()
        {
            string previousLanguageCode = Properties.Settings.Default.languageCode;
            Properties.Settings.Default.languageCode = SelectedLanguage.Code;
            Properties.Settings.Default.Save();

            return previousLanguageCode != SelectedLanguage.Code;
        }

        private void restoreAudioVolumes()
        {
            audioService.setMusicVolume(MusicVolume / VOLUME_PERCENTAGE_DIVISOR);
            audioService.setSoundEffectsVolume(SoundEffectsVolume / VOLUME_PERCENTAGE_DIVISOR);
        }

        private void promptForRestart()
        {
            bool shouldRestart = dialogService.showConfirmation(
                Lang.RestartRequiredMessage,
                Lang.RestartRequiredTitle);

            if (shouldRestart)
            {
                restartApplication();
            }
        }

        private void restartApplication()
        {
            try
            {
                Process.Start(Application.ResourceAssembly.Location);
                Application.Current.Shutdown();
            }
            catch (Exception)
            {
                dialogService.showError(Lang.ErrorRestartFailed, Lang.ErrorTitle);
            }
        }

        private void closeDialog(bool dialogResult)
        {
            setDialogResultAction?.Invoke(dialogResult);
            closeWindowAction?.Invoke();
        }
    }
}