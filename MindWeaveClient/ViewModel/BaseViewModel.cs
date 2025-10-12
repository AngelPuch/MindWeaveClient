using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows; // Necesario para acceder a 'Application' y 'CommandManager'

namespace MindWeaveClient.ViewModel
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // --- ESTE ES EL MÉTODO CLAVE ---
        // Fuerza a la interfaz de usuario a volver a preguntar si un comando se puede ejecutar.
        protected void OnCanExecuteChanged(ICommand command)
        {
            Application.Current.Dispatcher.Invoke(() => CommandManager.InvalidateRequerySuggested());
        }
    }
}