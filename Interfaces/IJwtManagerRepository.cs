using ServerlessLogin.Models;
using System.Security.Claims;

namespace ServerlessLogin.Interfaces
{
    public interface IJwtManagerRepository
    {
        Tokens GenerateJWT(string username);
        Tokens GenerateRefreshToken(string username);
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
