using FluentValidation;
using MindWeaveClient.Properties.Langs;
using MindWeaveClient.ViewModel.Authentication;

namespace MindWeaveClient.Validators
{
    public class VerificationValidator : AbstractValidator<VerificationViewModel>
    {
        public VerificationValidator()
        {
            RuleFor(vm => vm.VerificationCode)
                .NotEmpty().WithMessage(Lang.ValidationVerificationCodeRequired)
                .Length(6).WithMessage(Lang.ValidationVerificationCodeFormat)
                .Matches("^[0-9]{6}$").WithMessage(Lang.ValidationVerificationCodeFormat);
        }
    }
}
