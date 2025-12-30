using FluentValidation;
using MindWeaveClient.Properties.Langs;
using MindWeaveClient.ViewModel.Authentication;
using System;

namespace MindWeaveClient.Validators
{
    public class CreateAccountValidator : AbstractValidator<CreateAccountViewModel>
    {
        private const string PASSWORD_POLICY_REGEX = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#\\$%^&*(),.?\"":{}|<>]).{8,}$";
        private const int MAX_LENGTH_FIRST_NAME = 45;
        private const int MAX_LENGTH_LAST_NAME = 45;
        private const int MAX_LENGTH_USERNAME = 16;
        private const int MIN_LENGTH_USERNAME = 3;
        private const int MAX_LENGTH_EMAIL = 45;
        private const int MAX_LENGTH_PASSWORD = 128;

        public CreateAccountValidator()
        {
            RuleFor(vm => vm.FirstName)
                .NotEmpty().WithMessage(Lang.ValidationFirstNameRequired)
                .MaximumLength(MAX_LENGTH_FIRST_NAME);

            RuleFor(vm => vm.LastName)
                .NotEmpty().WithMessage(Lang.ValidationLastNameRequired)
                .MaximumLength(MAX_LENGTH_LAST_NAME);

            RuleFor(vm => vm.Username)
                .NotEmpty().WithMessage(Lang.ValidationUsernameRequired)
                .Length(MIN_LENGTH_USERNAME, MAX_LENGTH_USERNAME).WithMessage(Lang.ValidationUsernameLength);

            RuleFor(vm => vm.Email)
                .NotEmpty().WithMessage(Lang.ValidationEmailRequired)
                .EmailAddress().WithMessage(Lang.ValidationEmailInvalid)
                .MaximumLength(MAX_LENGTH_EMAIL);

            RuleFor(vm => vm.BirthDate)
                .NotNull().WithMessage(Lang.ValidationBirthDateRequired)
                .Must(beValidAge).WithMessage(Lang.ValidationBirthDateInvalid);

            RuleFor(vm => vm.Password)
                .NotEmpty().WithMessage(Lang.ValidationPasswordRequired)
                .MaximumLength(MAX_LENGTH_PASSWORD)
                .Matches(PASSWORD_POLICY_REGEX).WithMessage(Lang.ValidationPasswordPolicy);

            RuleFor(vm => vm.IsFemale) 
                .Must((vm, _) => vm.IsFemale || vm.IsMale || vm.IsOther || vm.IsPreferNotToSay)
                .WithMessage(Lang.ValidationGenderRequired)
                .When(vm => !vm.IsFemale && !vm.IsMale && !vm.IsOther && !vm.IsPreferNotToSay, ApplyConditionTo.CurrentValidator);
        }

        private static bool beValidAge(DateTime? birthDate)
        {
            if (!birthDate.HasValue) return false;

            var date = birthDate.Value.Date;
            var today = DateTime.Today;
            var minAgeDate = today.AddYears(-13);

            return date <= today && date <= minAgeDate;
        }
    }
}
