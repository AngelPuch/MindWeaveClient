using FluentValidation;
using FluentValidation.Results;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace MindWeaveClient.ViewModel
{
    public abstract class BaseViewModel : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool isBusy;
        public bool IsBusy
        {
            get => isBusy;
            protected set
            {
                isBusy = value;
                OnPropertyChanged();
                RaiseCanExecuteChangedOnCommands();
            }
        }

        protected void SetBusy(bool value)
        {
            IsBusy = value;
        }

        protected void RaiseCanExecuteChangedOnCommands()
        {
            Application.Current?.Dispatcher?.Invoke(() => CommandManager.InvalidateRequerySuggested());
        }

        // Sistema de errores
        private readonly Dictionary<string, List<string>> errors = new Dictionary<string, List<string>>();

        // Sistema de campos tocados
        private readonly HashSet<string> touchedProperties = new HashSet<string>();

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        // HasErrors ahora considera solo los campos tocados
        public bool HasErrors => errors.Any(e => touchedProperties.Contains(e.Key));

        /// <summary>
        /// Verifica si un campo específico tiene errores que deben mostrarse
        /// </summary>
        protected bool HasVisibleErrors(string propertyName)
        {
            return touchedProperties.Contains(propertyName) &&
                   errors.ContainsKey(propertyName) &&
                   errors[propertyName].Any();
        }

        public IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                return errors.Values.SelectMany(list => list);
            }

            // Solo devolver errores si el campo ha sido tocado
            if (touchedProperties.Contains(propertyName))
            {
                return errors.ContainsKey(propertyName) ? errors[propertyName] : null;
            }

            return null;
        }

        /// <summary>
        /// Marca una propiedad como "tocada" para empezar a mostrar sus errores
        /// </summary>
        protected void MarkAsTouched(string propertyName)
        {
            if (!touchedProperties.Contains(propertyName))
            {
                touchedProperties.Add(propertyName);
                OnErrorsChanged(propertyName);
            }
        }

        /// <summary>
        /// Marca todas las propiedades como tocadas (útil al intentar submit)
        /// </summary>
        protected void MarkAllAsTouched()
        {
            var allProperties = errors.Keys.ToList();
            foreach (var prop in allProperties)
            {
                MarkAsTouched(prop);
            }
        }

        /// <summary>
        /// Limpia el estado de touched (útil al resetear el formulario)
        /// </summary>
        protected void ClearTouchedState()
        {
            var propertiesToClear = touchedProperties.ToList();
            touchedProperties.Clear();

            foreach (var prop in propertiesToClear)
            {
                OnErrorsChanged(prop);
            }
        }

        protected void Validate<TViewModel>(IValidator<TViewModel> validator, TViewModel viewModel, string ruleSet = null)
            where TViewModel : class
        {
            ValidationResult validationResult;

            if (string.IsNullOrEmpty(ruleSet))
            {
                validationResult = validator.Validate(viewModel);
            }
            else
            {
                validationResult = validator.Validate(viewModel, v => v.IncludeRuleSets(ruleSet));
            }

            var propertyNamesWithErrors = errors.Keys.ToList();
            errors.Clear();

            if (!validationResult.IsValid)
            {
                foreach (var failure in validationResult.Errors)
                {
                    if (!errors.ContainsKey(failure.PropertyName))
                    {
                        errors[failure.PropertyName] = new List<string>();
                    }
                    errors[failure.PropertyName].Add(failure.ErrorMessage);
                }
            }

            var allAffectedProperties = propertyNamesWithErrors.Union(errors.Keys).Distinct();

            foreach (var propertyName in allAffectedProperties)
            {
                OnErrorsChanged(propertyName);
            }

            OnPropertyChanged(nameof(HasErrors));
        }

        protected void OnErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }
    }
}