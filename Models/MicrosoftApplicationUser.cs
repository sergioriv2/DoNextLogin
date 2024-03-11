using System.Security.Claims;

namespace ServerlessLogin.Models
{
    public class MicrosoftApplicationUser
    {
        public string Email { get; set; }
        public string Name { get; set; }

        public MicrosoftApplicationUser(ClaimsPrincipal claims)
        {
            Email = claims.FindFirst("preferred_username")?.Value;
            Name = claims.FindFirst("name")?.Value;
        }
    }
}
