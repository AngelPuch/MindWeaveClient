using FluentValidation;
using MindWeaveClient.Properties.Langs;
using MindWeaveClient.ViewModel.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
