using MindWeaveClient.Properties.Langs;

namespace MindWeaveClient.Utilities.Implementations
{
    public static class MessageCodeInterpreter
    {
        public static string translate(string messageCode, string fallbackMessage = "")
        {
            if (string.IsNullOrEmpty(messageCode))
            {
                return !string.IsNullOrEmpty(fallbackMessage) ? fallbackMessage : Lang.ErrorGeneric;
            }

            switch (messageCode)
            {
                case "AUTH_LOGIN_SUCCESS": return Lang.LoginSuccessful;
                case "AUTH_REGISTRATION_SUCCESS": return Lang.RegistrationSuccess;
                case "AUTH_VERIFICATION_SUCCESS": return Lang.VerificationSuccessful;
                case "AUTH_VERIFICATION_CODE_RESENT": return Lang.VerificationCodeResent;
                case "AUTH_RECOVERY_CODE_SENT": return Lang.RecoveryCodeSent;
                case "AUTH_PASSWORD_RESET_SUCCESS": return Lang.InfoPasswordResetSuccess;

                case "AUTH_INVALID_CREDENTIALS": return Lang.LoginInvalidCredentials;
                case "AUTH_USER_ALREADY_LOGGED_IN": return Lang.ErrorUserAlreadyLoggedIn;
                case "AUTH_ACCOUNT_NOT_VERIFIED": return Lang.LoginAccountNotVerified;
                case "AUTH_USER_ALREADY_EXISTS": return Lang.ErrorUserAlreadyExists;
                case "AUTH_USER_NOT_FOUND": return Lang.ErrorAccountNotFound;
                case "AUTH_ACCOUNT_ALREADY_VERIFIED": return Lang.VerificationAccountAlreadyVerified;
                case "AUTH_CODE_INVALID_OR_EXPIRED": return Lang.VerificationInvalidOrExpiredCode;

                case "VALIDATION_GENERAL_ERROR": return !string.IsNullOrEmpty(fallbackMessage) ? fallbackMessage : Lang.ErrorGeneric;
                case "VALIDATION_FIELDS_REQUIRED": return Lang.ErrorAllFieldsRequired;

                case "VALIDATION_EMAIL_REQUIRED": return Lang.ValidationEmailRequired;
                case "VALIDATION_EMAIL_CODE_REQUIRED": return Lang.VerificationEmailAndCodeRequired;
                case "VALIDATION_CODE_INVALID_FORMAT": return Lang.VerificationCodeInvalidFormat;
                case "VALIDATION_PROFILE_PASSWORD_REQUIRED": return Lang.ValidationProfileOrPasswordRequired;

                case "VALIDATION_PASSWORD_TOO_SHORT": return Lang.ValidationPasswordLength;
                case "VALIDATION_PASSWORD_TOO_WEAK": return Lang.ValidationPasswordComplexity;
                case "VALIDATION_PASSWORD_REQUIRED": return Lang.ValidationPasswordRequired;
                case "VALIDATION_PASSWORD_NO_SPACES": return Lang.ValidationPasswordNoSpaces;

                case "VALIDATION_USERNAME_REQUIRED": return Lang.ValidationUsernameRequired;
                case "VALIDATION_USERNAME_LENGTH": return Lang.ValidationUsernameLength;
                case "VALIDATION_USERNAME_ALPHANUMERIC": return Lang.ValidationUsernameAlphanumeric;

                case "VALIDATION_EMAIL_FORMAT": return Lang.ValidationEmailFormat;

                case "VALIDATION_NAME_LENGTH": return Lang.ValidationNameLength;
                case "VALIDATION_NAME_ONLY_LETTERS": return Lang.ValidationOnlyLetters;
                case "VALIDATION_NO_WHITESPACE": return Lang.ValidationNoLeadingOrTrailingWhitespace;

                case "VALIDATION_DATE_REQUIRED": return Lang.ValidationDateOfBirthRequired;
                case "VALIDATION_AGE_MINIMUM": return Lang.ValidationDateOfBirthMinimumAge;
                case "VALIDATION_AGE_REALISTIC": return Lang.ValidationDateOfBirthRealistic;

                case "ERROR_SERVER_GENERIC": return Lang.ErrorGeneric;
                case "ERROR_DATABASE": return Lang.ErrorDatabaseConnection;
                case "ERROR_COMMUNICATION_CHANNEL": return Lang.ErrorCommunication;
                case "ERROR_SERVICE_CLOSING": return Lang.ErrorServiceConnectionClosing;
                case "ERROR_SESSION_MISMATCH": return Lang.ErrorSessionMismatch;

                case "MATCH_USERNAME_REQUIRED": return Lang.ValidationUsernameRequired;
                case "MATCH_USER_ALREADY_BUSY": return Lang.ErrorUserAlreadyInGame;
                case "MATCH_LOBBY_NOT_FOUND": return Lang.ErrorLobbyNotFound;
                case "MATCH_USER_BANNED": return Lang.ErrorUserBanned;
                case "MATCH_LOBBY_FULL": return Lang.ErrorLobbyIsFull;
                case "MATCH_PLAYER_ALREADY_IN_LOBBY": return Lang.ErrorPlayerAlreadyInLobby;
                case "MATCH_USER_NOT_ONLINE": return Lang.ErrorUserNotOnline;
                case "MATCH_NOT_HOST": return Lang.ErrorNotHost;
                case "MATCH_NOT_ENOUGH_PLAYERS": return Lang.ErrorNotEnoughPlayers;
                case "MATCH_HOST_CANNOT_KICK_SELF": return Lang.ErrorHostCannotKickSelf;
                case "MATCH_PLAYER_NOT_FOUND": return Lang.ErrorPlayerNotFound;

                case "MATCH_LOBBY_CREATED": return Lang.SuccessLobbyCreated;
                case "MATCH_LOBBY_CREATION_FAILED": return Lang.ErrorLobbyCreationFailed;
                case "MATCH_JOIN_ERROR_DATA": return Lang.ErrorJoinLobbyData;
                case "MATCH_GUEST_JOIN_SUCCESS": return Lang.SuccessGuestJoined;
                case "MATCH_GUEST_NAME_GENERATION_FAILED": return Lang.ErrorGuestNameGeneration;
                case "MATCH_GUEST_INVITE_INVALID": return Lang.ErrorInvalidGuestInvite;
                case "MATCH_GUEST_INVITE_SEND_ERROR": return Lang.ErrorSendingGuestInvite;

                case "MATCH_START_DB_ERROR": return Lang.ErrorGameStartDatabase;
                case "MATCH_PUZZLE_FILE_NOT_FOUND": return Lang.ErrorPuzzleFileNotFound;
                case "MATCH_DIFFICULTY_CHANGE_ERROR": return Lang.ErrorDifficultyChange;
                case "MATCH_COMMUNICATION_ERROR": return Lang.ErrorCommunication;
                case "MATCH_NO_ACTIVE_CONNECTIONS": return Lang.ErrorNoActiveConnections;

                case "NOTIFY_KICKED_BY_HOST": return Lang.InfoKickedByHost;
                case "NOTIFY_KICKED_PROFANITY": return Lang.InfoKickedProfanity;
                case "NOTIFY_HOST_LEFT": return Lang.InfoHostLeftLobby;

                case "PROFILE_UPDATE_SUCCESS": return Lang.SuccessProfileUpdated;
                case "PROFILE_NOT_FOUND": return Lang.ErrorProfileNotFound;
                case "PROFILE_AVATAR_UPDATE_SUCCESS": return Lang.SuccessAvatarUpdated;
                case "PROFILE_PASSWORD_CHANGE_SUCCESS": return Lang.SuccessPasswordChanged;
                case "PROFILE_CURRENT_PASSWORD_INCORRECT": return Lang.ErrorCurrentPasswordIncorrect;

               
                case "SOCIAL_FRIEND_REQUEST_SENT": return Lang.SuccessFriendRequestSent;
                case "SOCIAL_FRIEND_REQUEST_ACCEPTED": return Lang.SuccessFriendRequestAccepted;
                case "SOCIAL_FRIEND_REQUEST_DECLINED": return Lang.SuccessFriendRequestDeclined;
                case "SOCIAL_FRIEND_REMOVED": return Lang.SuccessFriendRemoved;

                case "SOCIAL_ALREADY_FRIENDS": return Lang.ErrorAlreadyFriends;
                case "SOCIAL_REQUEST_ALREADY_SENT": return Lang.ErrorRequestAlreadySent;
                case "SOCIAL_REQUEST_ALREADY_RECEIVED": return Lang.ErrorRequestAlreadyReceived;
                case "SOCIAL_CANNOT_ADD_SELF": return Lang.ErrorCannotAddSelf;
                case "SOCIAL_USER_NOT_FOUND": return Lang.ErrorUserNotFound;
                case "SOCIAL_REQUEST_NOT_FOUND": return Lang.ErrorRequestNotFound;
                case "SOCIAL_NOT_FRIENDS": return Lang.ErrorNotFriends;

                case "PUZZLE_UPLOAD_SUCCESS": return Lang.SuccessPuzzleUploaded;
                case "PUZZLE_UPLOAD_FAILED": return Lang.ErrorPuzzleUploadFailed;
                case "PUZZLE_IMAGE_TOO_LARGE": return Lang.ErrorPuzzleImageTooLarge;
                case "PUZZLE_LOAD_FAILED": return Lang.ErrorPuzzleLoadFailed;
                case "MATCH_CANNOT_INVITE_BANNED": return Lang.WarningCannotInviteBaned;

                case "CHAT_PROFANITY_WARNING": return Lang.WarningProfanityDetected;

                default:
                    return !string.IsNullOrEmpty(fallbackMessage) ? fallbackMessage : Lang.ErrorGeneric;
            }
        }
    }
}