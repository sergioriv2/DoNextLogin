namespace ServerlessLogin.Models.Google
{
    public class ReCaptchaV3Response
    {
        public bool Success { get; set; }
        public string Challenge_ts { get; set; } = string.Empty;
        public string Hostname { get; set; } = string.Empty;
        public float Score { get; set; } = 0;
        public string Action { get; set; } = string.Empty;
    }
}