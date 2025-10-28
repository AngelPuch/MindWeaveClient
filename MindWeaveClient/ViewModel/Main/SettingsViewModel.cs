// MindWeaveClient/ViewModel/Main/SettingsViewModel.cs
using MindWeaveClient.Properties.Langs; // Para Lang
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input; // Para AudioManager

namespace MindWeaveClient.ViewModel.Main // Asegúrate que el namespace sea correcto
{
    public class LanguageOption
    {
        public string Name { get; set; }
        public string Code { get; set; }

        public override string ToString()
        {
            return Name; // Esto se mostrará en el ComboBox
        }
    }

    public class SettingsViewModel : BaseViewModel
    {
        // --- Backing Fields ---
        private double musicVolumeValue;
        private double soundEffectsVolumeValue;
        private LanguageOption selectedLanguageValue;
        private List<LanguageOption> availableLanguagesValue;

        // --- Public Properties (camelCase para binding) ---
        public double musicVolume
        {
            get => musicVolumeValue;
            set
            {
                musicVolumeValue = value;
                OnPropertyChanged();
                // Opcional: Aplicar en tiempo real mientras deslizas
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
                // Opcional: Aplicar en tiempo real mientras deslizas
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

        // --- Commands (camelCase) ---
        public ICommand saveCommand { get; }
        public ICommand cancelCommand { get; }
        // Se elimina el comando para 'Reload Progress'
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
                // Usa las claves de Lang.resx para los nombres mostrados
                new LanguageOption { Name = Lang.SettingsOptEnglish, Code = "en-US" },
                new LanguageOption { Name = Lang.SettingsOptSpanish, Code = "es-MX" }
            };

            // Seleccionar el idioma actual cargado
            string currentLangCode = Properties.Settings.Default.languageCode;
            selectedLanguage = availableLanguages.FirstOrDefault(lang => lang.Code == currentLangCode) ?? availableLanguages.First();
        }

        private void loadSettings()
        {
            // Carga valores desde Properties.Settings (crearemos estas propiedades más adelante)
            // Usamos valores por defecto si no existen aún.
            musicVolume = Properties.Settings.Default.MusicVolumeSetting; // Necesitarás crear esta propiedad
            soundEffectsVolume = Properties.Settings.Default.SoundEffectsVolumeSetting; // Necesitarás crear esta propiedad

            AudioManager.SetMusicVolume(musicVolume / 100.0);
            AudioManager.SetSoundEffectsVolume(soundEffectsVolume / 100.0);
        }

        private void executeSave()
        {
            // Guarda los valores en Properties.Settings (esto ya estaba)
            Properties.Settings.Default.MusicVolumeSetting = musicVolume;
            Properties.Settings.Default.SoundEffectsVolumeSetting = soundEffectsVolume;
            string previousLanguageCode = Properties.Settings.Default.languageCode;
            Properties.Settings.Default.languageCode = selectedLanguage.Code;
            Properties.Settings.Default.Save();

            // *** AQUÍ APLICAS EL VOLUMEN AL GUARDAR ***
            AudioManager.SetMusicVolume(musicVolume / 100.0);
            AudioManager.SetSoundEffectsVolume(soundEffectsVolume / 100.0);

            // Verifica si cambió el idioma (esto ya estaba)
            if (previousLanguageCode != selectedLanguage.Code)
            {
                applyLanguageChange();
            }

            // Cierra la ventana (esto ya estaba)
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
                "Restart Required", // Título - TODO: Añadir a Lang.resx
                MessageBoxButton.YesNo,
                MessageBoxImage.Information);

            if (result == MessageBoxResult.Yes)
            {
                // Código para reiniciar la aplicación
                System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                Application.Current.Shutdown();
            }
        }


        private void executeCancel()
        {
            // Cierra la ventana sin guardar (DialogResult = false por defecto o null)
            if (currentWindow != null)
            {
                currentWindow.DialogResult = false;
                currentWindow.Close();
            }
        }

        private void executeShowCredits()
        {
            // Lógica para mostrar créditos (ej. un nuevo MessageBox o ventana)
            MessageBox.Show("Mind Weave\nDeveloped by:\nAldo Antonio Campos Gómez\nAngel Jonathan Puch Hernández\n\n© 2025", "Credits"); // TODO: Añadir a Lang.resx
        }
    }
}