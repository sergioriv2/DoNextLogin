using ServerlessLogin.Dtos.Auth;
using ServerlessLogin.Models;

namespace ServerlessLogin.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> IsValidUser(string userId);
        Task<User> GetUserById(string userId);
        Task<User> GetUserByEmail(string email);
        Task<User> CreateUser(User user);
        Task<ValidationCode> CreateValidationCode(User user, int codeType, string code);
        string CreateRandomCode();
        Task<User> UpdateUser(User user);
        Task<User> DeleteUser(string userId);
        Task<bool> SaveRefreshToken(User user, Tokens token);
        Task<User> ActivateEmailCode(ActivateEmailDto activateEmailDto);
        Task<User> VerifyUserPassword(User user);
        Task<bool> Save();
        Task<bool> ValidateRefreshTokenExist(User user,  string refreshToken);
    }
}
