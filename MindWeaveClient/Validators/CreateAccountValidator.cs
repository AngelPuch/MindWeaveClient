using FluentValidation;
using MindWeaveClient.Properties.Langs;
using MindWeaveClient.ViewModel.Authentication;
using System;

namespace MindWeaveClient.Validators
{
    public class CreateAccountValidator : AbstractValidator<CreateAccountViewModel>
    {
        private const string PASSWORD_POLICY_REGEX = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#\\$%^&*(),.?\"":{}|<>]).{8,}$";

        public CreateAccountValidator()
        {
            RuleFor(vm => vm.FirstName)
                .NotEmpty().WithMessage(Lang.ValidationFirstNameRequired);

            RuleFor(vm => vm.LastName)
                .NotEmpty().WithMessage(Lang.ValidationLastNameRequired);

            RuleFor(vm => vm.Username)
                .NotEmpty().WithMessage(Lang.ValidationUsernameRequired)
                .Length(3, 16).WithMessage(Lang.ValidationUsernameLength);

            RuleFor(vm => vm.Email)
                .NotEmpty().WithMessage(Lang.ValidationEmailRequired)
                .EmailAddress().WithMessage(Lang.ValidationEmailInvalid);

            RuleFor(vm => vm.BirthDate)
                .NotNull().WithMessage(Lang.ValidationBirthDateRequired)
                .Must(beValidAge).WithMessage(Lang.ValidationBirthDateInvalid);

            RuleFor(vm => vm.Password)
                .NotEmpty().WithMessage(Lang.ValidationPasswordRequired)
                .Matches(PASSWORD_POLICY_REGEX).WithMessage(Lang.ValidationPasswordPolicy);

            RuleFor(vm => vm.IsFemale) 
                .Must((vm, _) => vm.IsFemale || vm.IsMale || vm.IsOther || vm.IsPreferNotToSay)
                .WithMessage(Lang.ValidationGenderRequired)
                .When(vm => !vm.IsFemale && !vm.IsMale && !vm.IsOther && !vm.IsPreferNotToSay, ApplyConditionTo.CurrentValidator);
        }

        private bool beValidAge(DateTime? birthDate)
        {
            if (!birthDate.HasValue) return false;

            var date = birthDate.Value.Date;
            var today = DateTime.Today;
            var minAgeDate = today.AddYears(-13);

            return date <= today && date <= minAgeDate;
        }
    }
}
