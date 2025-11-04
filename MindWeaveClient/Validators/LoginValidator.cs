using FluentValidation;
using MindWeaveClient.Properties.Langs;
using MindWeaveClient.ViewModel.Authentication;

namespace MindWeaveClient.Validators
{
    public class LoginValidator : AbstractValidator<LoginViewModel>
    {
        public LoginValidator()
        {
            RuleFor(vm => vm.Email)
                .NotEmpty().WithMessage(Lang.ValidationEmailRequired)
                .EmailAddress().WithMessage(Lang.ValidationEmailInvalid);

            RuleFor(vm => vm.Password)
                .NotEmpty().WithMessage(Lang.ValidationPasswordRequired);
        }
    }
}