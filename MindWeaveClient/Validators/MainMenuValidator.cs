using FluentValidation;
using MindWeaveClient.ViewModel.Main;
using MindWeaveClient.Properties.Langs;

namespace MindWeaveClient.Validators
{
    public class MainMenuValidator : AbstractValidator<MainMenuViewModel>
    {
        public MainMenuValidator()
        {
            RuleSet("JoinLobby", () =>
            {
                RuleFor(vm => vm.JoinLobbyCode)
                    .NotEmpty().WithMessage(Lang.ValidationLobbyCodeRequired)
                    .Length(6).WithMessage(Lang.ValidationLobbyCodeFormat)
                    .Matches("^[a-zA-Z0-9]{6}$").WithMessage(Lang.ValidationLobbyCodeFormat);
            });
        }
    }
}
