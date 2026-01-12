using FluentValidation;
using MindWeaveClient.ViewModel.Main;
using MindWeaveClient.Properties.Langs;

namespace MindWeaveClient.Validators
{
    public class MainMenuValidator : AbstractValidator<MainMenuViewModel>
    {
        private const string JOIN_LOBBY = "JoinLobby";
        private const int LOBBY_CODE_LENGTH = 6;
        private const string LOBBY_CODE_REGEX = "^[a-zA-Z0-9]{6}$";

        public MainMenuValidator()
        {
            RuleSet(JOIN_LOBBY, () =>
            {
                RuleFor(vm => vm.JoinLobbyCode)
                    .NotEmpty().WithMessage(Lang.ValidationLobbyCodeRequired)
                    .Length(LOBBY_CODE_LENGTH).WithMessage(Lang.ValidationLobbyCodeFormat)
                    .Matches(LOBBY_CODE_REGEX).WithMessage(Lang.ValidationLobbyCodeFormat);
            });
        }
    }
}
