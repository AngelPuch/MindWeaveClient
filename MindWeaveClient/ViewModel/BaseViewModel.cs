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

        private readonly Dictionary<string, List<string>> errors = new Dictionary<string, List<string>>();

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public bool HasErrors => errors.Any();

        public IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                return errors.Values.SelectMany(list => list);
            }
            return errors.ContainsKey(propertyName) ? errors[propertyName] : null;
        }

        protected void Validate<TViewModel>(FluentValidation.IValidator<TViewModel> validator, TViewModel viewModel)
        {
            var validationResult = validator.Validate(viewModel);

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

            // Raise for HasErrors property
            OnPropertyChanged(nameof(HasErrors));
        }

        protected void OnErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }
    }
}