using Microsoft.IdentityModel.Tokens;
using ServerlessLogin.Models;
using System.Security.Claims;

namespace ServerlessLogin.Interfaces
{
    public interface IJwtManagerRepository
    {
        Tokens GenerateJWT(string username);
        Tokens GenerateRefreshToken(string username);
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);

        Task<SecurityKey> GetIssuerSigningKey(string jwksUri, string kid);

        Task <MicrosoftApplicationUser> ValidateMicrosoftToken(string token);
    }
}
