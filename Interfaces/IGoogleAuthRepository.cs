

using Google.Apis.Auth;

namespace ServerlessLogin.Interfaces
{
    public interface IGoogleAuthRepository
    {
        Task<GoogleJsonWebSignature.Payload> VerifyGoogleToken(string token);
    }
}