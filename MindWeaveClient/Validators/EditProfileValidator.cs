using FluentValidation;
using MindWeaveClient.Properties.Langs;
using MindWeaveClient.ViewModel.Main;
using System;

namespace MindWeaveClient.Validators
{
    public class EditProfileValidator : AbstractValidator<EditProfileViewModel>
    {
        private const string PROFILE = "Profile";
        private const string PASSWORD = "Password";
        private const string CREDENTIAL_POLICY_REGEX = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#\\$%^&*(),.?\"":{}|<>]).{8,}$";
        private const string NAME_VALIDATOR_REGEX = @"^(?=.*\p{L})[\p{L}\p{M} '\-\.]+$";

        private const string SOCIAL_USERNAME_REGEX = @"^[a-zA-Z0-9._-]+$";
        private const int MAX_SOCIAL_USERNAME_LENGTH = 100;
        private const int MAX_NAME_LENGTH = 45;
        private const int MIN_NAME_LENGTH = 3;
        private const int MAX_PASSWORD_INPUT_LENGTH = 128;
        private const int MIN_AGE = -13;

        public EditProfileValidator()
        {
            RuleSet(PROFILE, () =>
            {
                RuleFor(x => x.FirstName)
                    .NotEmpty().WithMessage(Lang.ValidationFirstNameRequired)
                    .Length(MIN_NAME_LENGTH, MAX_NAME_LENGTH).WithMessage(Lang.ValidationFirstNameLength)
                    .Matches(NAME_VALIDATOR_REGEX).WithMessage(Lang.ValidationNameInvalidCharacters);

                RuleFor(x => x.LastName)
                    .NotEmpty().WithMessage(Lang.ValidationLastNameRequired)
                    .Length(MIN_NAME_LENGTH, MAX_NAME_LENGTH).WithMessage(Lang.ValidationLastNameLength)
                    .Matches(NAME_VALIDATOR_REGEX).WithMessage(Lang.ValidationNameInvalidCharacters);

                RuleFor(x => x.DateOfBirth)
                    .NotNull().WithMessage(Lang.ValidationBirthDateRequired)
                    .LessThan(DateTime.Now.AddYears(MIN_AGE)).WithMessage(Lang.ValidationBirthDateInvalid);
            });

            RuleForEach(x => x.SocialMediaList).ChildRules(socialMedia =>
            {
                socialMedia.RuleFor(sm => sm.Username)
                    .NotEmpty().WithMessage(Lang.ValidationSocialUsernameRequired) // Usar el nuevo mensaje específico
                    .MaximumLength(MAX_SOCIAL_USERNAME_LENGTH).WithMessage(Lang.ValidationSocialUsernameLength) // Usar el nuevo mensaje de 100 chars
                    .Matches(SOCIAL_USERNAME_REGEX).WithMessage(Lang.ValidationSocialUsernameInvalid); // Usar el nuevo mensaje específico
            });

            RuleSet(PASSWORD, () =>
            {
                RuleFor(x => x.CurrentPassword)
                    .NotEmpty().WithMessage(Lang.ValidationCurrentPasswordRequired)
                    .MaximumLength(MAX_PASSWORD_INPUT_LENGTH);

                RuleFor(x => x.NewPassword)
                    .NotEmpty().WithMessage(Lang.ValidationPasswordRequired)
                    .MaximumLength(MAX_PASSWORD_INPUT_LENGTH)
                    .Matches(CREDENTIAL_POLICY_REGEX).WithMessage(Lang.ValidationPasswordPolicy);

                RuleFor(x => x.ConfirmPassword)
                    .Equal(x => x.NewPassword).WithMessage(Lang.ValidationPasswordsDoNotMatch)
                    .MaximumLength(MAX_PASSWORD_INPUT_LENGTH);
            });


        }
    }
}