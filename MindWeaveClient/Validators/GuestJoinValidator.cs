using FluentValidation;
using MindWeaveClient.Properties.Langs;
using MindWeaveClient.ViewModel.Authentication;

namespace MindWeaveClient.Validators
{
    public class GuestJoinValidator : AbstractValidator<GuestJoinViewModel>
    {
        public GuestJoinValidator()
        {
            RuleFor(vm => vm.LobbyCode)
                .NotEmpty().WithMessage(Lang.ValidationLobbyCodeRequired)
                .Length(6).WithMessage(Lang.ValidationLobbyCodeFormat)
                .Matches("^[a-zA-Z0-9]{6}$").WithMessage(Lang.ValidationLobbyCodeFormat);

            RuleFor(vm => vm.GuestEmail)
                .NotEmpty().WithMessage(Lang.ValidationEmailRequired)
                .EmailAddress().WithMessage(Lang.ValidationEmailInvalid);

            RuleFor(vm => vm.DesiredUsername)
                .NotEmpty().WithMessage(Lang.ValidationUsernameRequired)
                .Matches("^[a-zA-Z0-9]{3,16}$").WithMessage(Lang.ValidationUsernameLength);
        }
    }
}
