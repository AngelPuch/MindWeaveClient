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

        protected static void RaiseCanExecuteChangedOnCommands()
        {
            Application.Current?.Dispatcher?.Invoke(CommandManager.InvalidateRequerySuggested);
        }

        private readonly Dictionary<string, List<string>> errors = new Dictionary<string, List<string>>();
        private readonly HashSet<string> touchedProperties = new HashSet<string>();
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public bool HasErrors => errors.Any(e => touchedProperties.Contains(e.Key));

        protected bool hasVisibleErrors(string propertyName)
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

            if (touchedProperties.Contains(propertyName) && errors.TryGetValue(propertyName, out var errorList))
            {
                return errorList;
            }

            return Enumerable.Empty<string>();
        }

        protected void markAsTouched(string propertyName)
        {
            if (touchedProperties.Add(propertyName))
            {
                OnErrorsChanged(propertyName);
            }
        }

        protected void markAllAsTouched()
        {
            var allProperties = errors.Keys.ToList();
            foreach (var prop in allProperties)
            {
                markAsTouched(prop);
            }
        }

        protected void clearTouchedState()
        {
            var propertiesToClear = touchedProperties.ToList();
            touchedProperties.Clear();

            foreach (var prop in propertiesToClear)
            {
                OnErrorsChanged(prop);
            }
        }

        protected void validate<TViewModel>(IValidator<TViewModel> validator, TViewModel viewModel, string ruleSet = null)
            where TViewModel : class
        {
            var validationResult = string.IsNullOrEmpty(ruleSet) ? 
                validator.Validate(viewModel) : validator.Validate(viewModel, v => v.IncludeRuleSets(ruleSet));

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