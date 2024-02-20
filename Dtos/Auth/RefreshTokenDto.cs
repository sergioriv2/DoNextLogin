using FluentValidation;

namespace ServerlessLogin.Dtos.Auth
{
    public class RefreshTokenDto
    {
        public string RefreshToken { get; set; } = string.Empty;
    }

    public class RefreshTokenResponseDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }

    public class RefreshTokenDtoValidation : AbstractValidator<RefreshTokenDto> {

        public RefreshTokenDtoValidation()
        {
            RuleFor(x => x.RefreshToken).NotEmpty().NotNull();
        }
    }
}
