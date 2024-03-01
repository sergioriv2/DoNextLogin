namespace ServerlessLogin.Filters
{
    public class CustomValidationException : Exception
    {
        public CustomValidationCodes ExceptionCode { get; set; }
        public string Message { get; set; } = string.Empty;

        public CustomValidationException(CustomValidationCodes exceptionCode) : base()
        {
            Message = CustomValidationExceptionsDictionary.Messages[CustomValidationCodes.EmailAlreadyOnUse];
            ExceptionCode = exceptionCode;
        }
    }

    public enum CustomValidationCodes
    {
        // Auth -- 100x
        EmailAlreadyOnUse = 1001,
        PasswordsDoesntMatch = 1002,
        InvalidEmail = 1003,
        InvalidEmailCode = 1004,
        InvalidRefreshToken = 1005,
        ExpiredAccessOrRefreshToken = 1006,
        InvalidGoogleToken = 1007,
        ErrorValidatingReCaptcha = 1008,
        LowOrUnsuccessfullReCaptchaScore = 1009,
        // User -- 200x
        UserNotFound = 2001,
    }

    public static class CustomValidationExceptionsDictionary
    {
        public static readonly Dictionary<CustomValidationCodes, string> Messages = new Dictionary<CustomValidationCodes, string>()
        {
            {
                CustomValidationCodes.EmailAlreadyOnUse, "Email already on use."
            },
            {
                CustomValidationCodes.PasswordsDoesntMatch, "The credentials are invalid."
            },
            {
                CustomValidationCodes.InvalidEmail, "The credentials are invalid."
            },
            {
                CustomValidationCodes.InvalidEmailCode, "The code sent is invalid."
            },
            {
                CustomValidationCodes.UserNotFound, "User not found."
            },
            {
                CustomValidationCodes.InvalidRefreshToken, "Invalid refresh token or expired."
            },
            {
                 CustomValidationCodes.ExpiredAccessOrRefreshToken, "Expired refresh token or access token."
            },
            {
                 CustomValidationCodes.InvalidGoogleToken, "Invalid google token."
            },
            {
                 CustomValidationCodes.LowOrUnsuccessfullReCaptchaScore, "The score from the recaptcha score is low or the validation was unsuccessful."
            },
            {
                 CustomValidationCodes.ErrorValidatingReCaptcha, "There was an error while attempting to validate the recaptcha code."
            }
        };
    }
}
