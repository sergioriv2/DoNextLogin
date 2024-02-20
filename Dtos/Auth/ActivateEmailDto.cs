using FluentValidation;

namespace ServerlessLogin.Dtos.Auth
{
    public class ActivateEmailDto
    {
        public string Email { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
    }

    public class ActivateEmailResponseDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;


    }

    public class ActivateEmailDtoValidation : AbstractValidator<ActivateEmailDto>
    {
        public ActivateEmailDtoValidation()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress().NotEmpty();
            RuleFor(x => x.Code).NotEmpty().NotEmpty().MinimumLength(4).MaximumLength(4);
        }
    }
}
