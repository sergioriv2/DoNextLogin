using System.Text;
using Google.Apis.Auth;
using ServerlessLogin.Filters;
using ServerlessLogin.Interfaces;
using ServerlessLogin.Models.Google;

namespace ServerlessLogin.Repositories
{
    public class GoogleAuthRepository : IGoogleAuthRepository
    {

        private readonly IConfiguration _configuration;

        public GoogleAuthRepository(
            IConfiguration configuration
        )
        {
            _configuration = configuration;
        }

        public async Task<bool> VerifyReCaptchaV3Token(string token)
        {
            string secretKey = _configuration.GetValue<string>("Google:reCaptchaV3Secret");
            var reCaptchaRequest = new HttpClient()
            {
                BaseAddress = new Uri("https://www.google.com")
            };
            try
            {
                using var response = await
                    reCaptchaRequest
                    .GetAsync($"/recaptcha/api/siteverify?secret={secretKey}&response={token}");

                response.EnsureSuccessStatusCode();
                var textResponse = await response.Content.ReadAsStringAsync();
                var jsonResponse = await response.Content.ReadFromJsonAsync<ReCaptchaV3Response>() ?? throw new Exception();

                if (!jsonResponse.Success && jsonResponse.Score < 0.7)
                {
                    Console.Write("Text response: " + textResponse);
                    throw new CustomValidationException(CustomValidationCodes.LowOrUnsuccessfullReCaptchaScore);
                }
                else
                {
                    return true;
                }

            }
            catch (CustomValidationException)
            {
                throw;
            }
            catch
            {
                Console.Write("Error verifying recaptcha token");
                throw new CustomValidationException(CustomValidationCodes.ErrorValidatingReCaptcha);
            }


            return true;
        }


        public async Task<GoogleJsonWebSignature.Payload> VerifyGoogleToken(string token)
        {
            GoogleJsonWebSignature.ValidationSettings settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new List<string>()
                {
                    "808733031898-ku5qok1ru2kk0uvhej3e14of2ih2gqka.apps.googleusercontent.com"
                }
            };

            try
            {
                GoogleJsonWebSignature.Payload payload = await GoogleJsonWebSignature.ValidateAsync(token, settings);
                return payload;
            }
            catch (Exception ex)
            {
                Console.Write(ex);
                throw new CustomValidationException(CustomValidationCodes.InvalidGoogleToken);
            }
        }
    }
}