using FluentValidation;
using MindWeaveClient.Properties.Langs;
using MindWeaveClient.ViewModel.Authentication;

namespace MindWeaveClient.Validators
{
    public class PasswordRecoveryValidator : AbstractValidator<PasswordRecoveryViewModel>
    {
        private const string PASSWORD_POLICY_REGEX = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#\\$%^&*(),.?\"":{}|<>]).{8,}$";

        public PasswordRecoveryValidator()
        {
            RuleSet("Step1", () =>
            {
                RuleFor(vm => vm.Email)
                    .NotEmpty().WithMessage(Lang.ValidationEmailRequired)
                    .EmailAddress().WithMessage(Lang.ValidationEmailInvalid);
            });

            RuleSet("Step2", () =>
            {
                RuleFor(vm => vm.VerificationCode)
                    .NotEmpty().WithMessage(Lang.ValidationVerificationCodeRequired)
                    .Length(6).WithMessage(Lang.ValidationVerificationCodeFormat)
                    .Matches("^[0-9]{6}$").WithMessage(Lang.ValidationVerificationCodeFormat);
            });

            RuleSet("Step3", () =>
            {
                RuleFor(vm => vm.NewPassword)
                    .NotEmpty().WithMessage(Lang.ValidationPasswordNewRequired)
                    .Matches(PASSWORD_POLICY_REGEX).WithMessage(Lang.ValidationPasswordPolicy);

                RuleFor(vm => vm.ConfirmPassword)
                    .NotEmpty().WithMessage(Lang.ValidationPasswordConfirmRequired)
                    .Equal(vm => vm.NewPassword).WithMessage(Lang.ValidationPasswordsDoNotMatch);
            });
        }
    }
}