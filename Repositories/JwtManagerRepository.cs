using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using ServerlessLogin.Data;
using ServerlessLogin.Filters;
using ServerlessLogin.Interfaces;
using ServerlessLogin.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ServerlessLogin.Repositories
{
    public class JwtManagerRepository : IJwtManagerRepository
    {
        private readonly IConfiguration _configuration;

        public JwtManagerRepository(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        public Tokens GenerateTokens(string username)
        {
            try
            {
                var date = DateTime.UtcNow;
                var tokenHandler = new JwtSecurityTokenHandler();

                var accessTokenKey = Encoding.UTF8.GetBytes(_configuration["JWT:AccessKey"]);
                var refreshTokenKey = Encoding.UTF8.GetBytes(_configuration["JWT:RefreshKey"]);

                //Console.WriteLine("accessTokenKey: " + _configuration["JWT:AccessKey"]);
                //Console.WriteLine("refreshTokenKey: " + _configuration["JWT:RefreshKey"]);

                var accessTokenDescriptor = new SecurityTokenDescriptor()
                {
                    Subject = new ClaimsIdentity(
                            new Claim[]
                            {
                                new Claim(ClaimTypes.Name, username)
                            }
                        ),
                    Expires = date.AddMinutes(10),
                    NotBefore = date,
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(accessTokenKey), SecurityAlgorithms.HmacSha256Signature)
                };

                var refreshTokenDescriptor = new SecurityTokenDescriptor()
                {
                    Subject = new ClaimsIdentity(
                            new Claim[]
                            {
                                new Claim(ClaimTypes.Name, username)
                            }
                        ),
                    Expires = date.AddDays(7),
                    NotBefore = date,
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(refreshTokenKey), SecurityAlgorithms.HmacSha256Signature)
                };

                var accessToken = tokenHandler.CreateToken(accessTokenDescriptor);
                var refreshToken = tokenHandler.CreateToken(refreshTokenDescriptor);

                return new Tokens()
                {
                    AccessToken = tokenHandler.WriteToken(accessToken),
                    RefreshToken = tokenHandler.WriteToken(refreshToken),
                };

            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex);
                return null;
            }
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public Tokens GenerateJWT(string username)
        {
            return GenerateTokens(username);
        }

        public Tokens GenerateRefreshToken(string username)
        {
            return GenerateTokens(username);
        }

        public async Task<SecurityKey> GetIssuerSigningKey(string jwksUri, string kid)
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.GetStringAsync(jwksUri);

            // Parse JWKS to JSON
            var jwks = JsonConvert.DeserializeObject<JsonWebKeySet>(response);
            var webKey = jwks.Keys.Where(x => x.Kid == kid).First() ?? throw new Exception("Error obtaining KID");

            var signingKey = new JsonWebKey()
            {
                Kty = webKey.Kty,
                Kid = webKey.Kid,
                Use = webKey.Use,
                Alg = webKey.Alg,
                K = webKey.K,
                X = webKey.X,
                Y = webKey.Y,
                N = webKey.N,
                E = webKey.E,
                P = webKey.P,
                Q = webKey.Q,
                DP = webKey.DP,
                DQ = webKey.DQ,
                QI = webKey.QI,
                X5t = webKey.X5t,
                X5u = webKey.X5u,
            };

            return signingKey;
        }

        public async Task<MicrosoftApplicationUser> ValidateMicrosoftToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            var headers = jwtToken.Header.ToDictionary(k => k.Key, v => v.Value);

            var issuerSigningKey = await GetIssuerSigningKey(
                $"https://login.microsoftonline.com/{_configuration["AzureAd:TenantId"]}/discovery/v2.0/keys",
                 headers.TryGetValue("kid", out var kid) ? kid.ToString() : ""
                );

            if (issuerSigningKey == null)
            {
                throw new Exception("Error at: Issuer Signing Key");
            }

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = issuerSigningKey,
                ValidateIssuer = true,
                ValidIssuer = $"https://login.microsoftonline.com/{_configuration["AzureAd:TenantId"]}/v2.0",
                ValidateAudience = true,
                ValidAudience = _configuration["AzureAd:ClientId"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                var claimsPrincipal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                var microsoftClaims = new MicrosoftApplicationUser(claimsPrincipal);

                return microsoftClaims;
            }
            catch
            {
                throw new Exception("Error at: Authenticating microsoft token");
            }
        }


        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            try
            {
                var key = Encoding.UTF8.GetBytes(_configuration["JWT:RefreshKey"]);

                var tokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var principal = tokenHandler.ValidateToken(
                        token,
                        tokenValidationParameters,
                        out SecurityToken securityToken
                );

                JwtSecurityToken jwtSecurityToken = securityToken as JwtSecurityToken;
                if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new SecurityTokenException("Invalid token");
                }

                return principal;
            }
            catch (SecurityTokenException)
            {
                throw new CustomValidationException(CustomValidationCodes.InvalidRefreshToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
