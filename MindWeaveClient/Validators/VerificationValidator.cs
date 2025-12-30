using FluentValidation;
using MindWeaveClient.Properties.Langs;
using MindWeaveClient.ViewModel.Authentication;

namespace MindWeaveClient.Validators
{
    public class VerificationValidator : AbstractValidator<VerificationViewModel>
    {
        private const string VERIFICATION_CODE_REGEX = "^[0-9]{6}$";
        private const int VERIFICATION_CODE_LENGTH = 6;

        public VerificationValidator()
        {
            RuleFor(vm => vm.VerificationCode)
                .NotEmpty().WithMessage(Lang.ValidationVerificationCodeRequired)
                .Length(VERIFICATION_CODE_LENGTH).WithMessage(Lang.ValidationVerificationCodeFormat)
                .Matches(VERIFICATION_CODE_REGEX).WithMessage(Lang.ValidationVerificationCodeFormat);
        }
    }
}
