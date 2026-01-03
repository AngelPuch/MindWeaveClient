using MindWeaveClient.Properties.Langs;

namespace MindWeaveClient.Helpers
{
    public static class MessageCodeInterpreter
    {
        public static string Translate(string messageCode, string fallbackMessage = "")
        {
            if (string.IsNullOrEmpty(messageCode))
            {
                return !string.IsNullOrEmpty(fallbackMessage) ? fallbackMessage : Lang.ErrorGeneric;
            }

            switch (messageCode)
            {
                case "AUTH_LOGIN_SUCCESS":
                    return Lang.LoginSuccessful;
                case "AUTH_REGISTRATION_SUCCESS":
                    return Lang.RegistrationSuccess;
                case "AUTH_VERIFICATION_SUCCESS":
                    return Lang.VerificationSuccessful;
                case "AUTH_VERIFICATION_CODE_RESENT":
                    return Lang.RegistrationSuccessful; 
                    return Lang.InfoRecoveryCodeSent;
                case "AUTH_PASSWORD_RESET_SUCCESS":
                    return Lang.InfoPasswordResetSuccess;

                case "AUTH_INVALID_CREDENTIALS":
                    return Lang.LoginInvalidCredentials; 
                case "AUTH_ACCOUNT_NOT_VERIFIED":
                    return Lang.LoginAccountNotVerified;
                case "AUTH_USER_ALREADY_EXISTS":
                    return Lang.ErrorUserAlreadyExists;
                case "AUTH_USER_NOT_FOUND":
                    return Lang.ErrorAccountNotFound;
                case "AUTH_ACCOUNT_ALREADY_VERIFIED":
                    return Lang.VerificationAccountAlreadyVerified;
                case "AUTH_CODE_INVALID_OR_EXPIRED":
                    return Lang.VerificationInvalidOrExpiredCode;

                case "VALIDATION_GENERAL_ERROR":
                  
                    return !string.IsNullOrEmpty(fallbackMessage) ? fallbackMessage : Lang.ErrorGeneric;

                case "VALIDATION_FIELDS_REQUIRED":
                    return Lang.ErrorAllFieldsRequired;
                case "VALIDATION_EMAIL_REQUIRED":
                    return Lang.ValidationEmailRequired;
                case "VALIDATION_EMAIL_CODE_REQUIRED":
                    return Lang.VerificationEmailAndCodeRequired;
                case "VALIDATION_CODE_INVALID_FORMAT":
                    return Lang.VerificationCodeInvalidFormat;
                case "VALIDATION_PROFILE_PASSWORD_REQUIRED":
                    return Lang.ValidationProfileOrPasswordRequired;

                case "ERROR_DATABASE":
                    return Lang.ErrorDatabaseConnection;
                case "ERROR_SERVER_GENERIC":
                    return Lang.ErrorGeneric;

                case "MATCH_USERNAME_REQUIRED": return Lang.ValidationUsernameRequired;
                case "MATCH_USER_ALREADY_BUSY": return Lang.ErrorUserAlreadyInGame;
                case "MATCH_LOBBY_NOT_FOUND": return Lang.ErrorLobbyNotFound;
                case "MATCH_LOBBY_FULL": return Lang.ErrorLobbyIsFull;
                case "MATCH_PLAYER_ALREADY_IN_LOBBY": return Lang.ErrorPlayerAlreadyInLobby;
                case "MATCH_USER_NOT_ONLINE": return Lang.ErrorUserNotOnline;
                case "MATCH_NOT_HOST": return Lang.ErrorNotHost;
                case "MATCH_NOT_ENOUGH_PLAYERS": return Lang.ErrorNotEnoughPlayers;
                case "MATCH_HOST_CANNOT_KICK_SELF": return Lang.ErrorHostCannotKickSelf;
                case "MATCH_LOBBY_CREATION_FAILED": return Lang.ErrorLobbyCreationFailed;
                case "MATCH_GUEST_JOIN_SUCCESS": return Lang.SuccessGuestJoined;
                case "MATCH_GUEST_INVITE_INVALID": return Lang.ErrorInvalidGuestInvite;
                case "MATCH_PUZZLE_FILE_NOT_FOUND": return Lang.ErrorPuzzleNotFound;
                case "MATCH_COMMUNICATION_ERROR": return Lang.ErrorCommunication;

                
                case "NOTIFY_KICKED_BY_HOST": return Lang.InfoKickedByHost;
                case "NOTIFY_HOST_LEFT": return Lang.InfoHostLeftLobby;









                default:
                    return Lang.ErrorGeneric;
            }
        }
    }
}