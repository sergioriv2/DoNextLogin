using Google.Apis.Auth;
using ServerlessLogin.Filters;
using ServerlessLogin.Interfaces;

namespace ServerlessLogin.Repositories
{
    public class GoogleAuthRepository : IGoogleAuthRepository
    {
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