using FluentValidation;
using MindWeaveClient.Properties.Langs;
using MindWeaveClient.ViewModel.Main;
using System;

namespace MindWeaveClient.Validators
{
    public class EditProfileValidator : AbstractValidator<EditProfileViewModel>
    {
        private const string PASSWORD_POLICY_REGEX = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#\\$%^&*(),.?\"":{}|<>]).{8,}$";

        public EditProfileValidator()
        {
            RuleSet("Profile", () =>
            {
                RuleFor(x => x.FirstName)
                    .NotEmpty().WithMessage(Lang.ValidationFirstNameRequired);

                RuleFor(x => x.LastName)
                    .NotEmpty().WithMessage(Lang.ValidationLastNameRequired);

                RuleFor(x => x.DateOfBirth)
                    .NotNull().WithMessage(Lang.ValidationBirthDateRequired)
                    .LessThan(DateTime.Now.AddYears(-13)).WithMessage(Lang.ValidationBirthDateInvalid);

                RuleFor(x => x.SelectedGender)
                    .NotNull().WithMessage(Lang.ValidationGenderRequired);
            });

            RuleSet("Password", () =>
            {
                RuleFor(x => x.CurrentPassword)
                    .NotEmpty().WithMessage(Lang.ValidationCurrentPasswordRequired);

                RuleFor(x => x.NewPassword)
                    .NotEmpty().WithMessage(Lang.ValidationPasswordRequired)
                    .Matches(PASSWORD_POLICY_REGEX).WithMessage(Lang.ValidationPasswordPolicy);

                RuleFor(x => x.ConfirmPassword)
                    .Equal(x => x.NewPassword).WithMessage(Lang.ValidationPasswordsDoNotMatch);
            });
        }
    }
}