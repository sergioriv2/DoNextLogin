using ServerlessLogin.Models;

namespace ServerlessLogin.Interfaces
{
    public interface IEmailRepository
    {
        Task <bool> SendValidationCodeEmail(User user, string validationCode);
    }
}
