namespace ServerlessLogin.Dtos.Auth
{
    public class ValidateGoogleJWTDto
    {
        public string token { get; set; } = string.Empty;
    }

    public class ValidateReCaptchaResponseDto
    {
        public bool Success { get; set; }
    }
}