using FluentValidation;

namespace ServerlessLogin.Dtos.Auth
{
    public class SignupDto
    {
        public string? FirstName { get; set; } = string.Empty;
        public string? LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class SignupResponseDto
    {
        public bool EmailSent { get; set; }
    }

    public class SignupDtoValidation : AbstractValidator<SignupDto>
    {
        public SignupDtoValidation() {
            string passwordRegex = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d]).{8,}$";

            RuleFor(x => x.Email).EmailAddress().NotNull().NotEmpty();
            RuleFor(x => x.Password).Matches(passwordRegex).NotNull().NotEmpty();
        }    
    }

}
