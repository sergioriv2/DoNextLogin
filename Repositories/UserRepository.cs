using Microsoft.EntityFrameworkCore;
using ServerlessLogin.Data;
using ServerlessLogin.Dtos.Auth;
using ServerlessLogin.Filters;
using ServerlessLogin.Interfaces;
using ServerlessLogin.Models;
using System.Net.Mail;
using static Status;

namespace ServerlessLogin.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _dataContext;
        private readonly DbSet<User> _usersContext;
        private readonly DbSet<ValidationCode> _validationCodesContext;
        private readonly DbSet<RefreshToken> _refreshTokensContext;

        public UserRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
            _usersContext = dataContext.Users;
            _validationCodesContext = dataContext.ValidationCodes;
            _refreshTokensContext = dataContext.RefreshTokens;
        }

        

        public async Task<bool> SaveRefreshToken(User user, Tokens token)
        {
            var newRefreshToken = new RefreshToken()
            {
                Token = token.RefreshToken,
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };

            await _refreshTokensContext.AddAsync(newRefreshToken);
            
            return true;
        }
        public async Task<User> CreateUser(User user)
        {
            try
            {
                var userExist = await GetUserByEmail(user.Email);

                if (userExist != null)
                {
                    throw new CustomValidationException(CustomValidationCodes.EmailAlreadyOnUse);
                }

                user.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(user.Password);
                await _usersContext.AddAsync(user);

                return user;
            }
            catch (CustomValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

        }

        public async Task<User> GetUserByEmail(string email)
        {
            return await _usersContext.Where(user => user.Email == email).FirstOrDefaultAsync();
        }

        public async Task<bool> Save()
        {
            await _dataContext.SaveChangesAsync();
            return true;
        }

        public async Task<User> DeleteUser(string userId)
        {
            var user = await GetUserById(userId);
            _usersContext.Remove(user);
            return user;
        }

        public async Task<User> GetUserById(string userId)
        {
            return await _usersContext.Where(user => user.Id == userId).FirstOrDefaultAsync();
        }

        public async Task<bool> IsValidUser(string userId)
        {
            return await _usersContext.AnyAsync(user => user.Id == userId);
        }

        public Task<User> UpdateUser(User user)
        {
            throw new NotImplementedException();
        }

        public async Task<ValidationCode> CreateValidationCode(User user, int codeType, string code)
        {
            DateTime now = DateTime.UtcNow;
            ValidationCode validationCode = new()
            {

                Code = BCrypt.Net.BCrypt.HashPassword(code),
                ExpiresAt = now.AddMinutes(30),
                UserId = user.Id,
                CodeTypeId = codeType.ToString()
            };

            await _validationCodesContext.AddAsync(validationCode);
            return validationCode;
        }

        public string CreateRandomCode()
        {
            int randomInt = new Random().Next(1000, 10000);
            return randomInt.ToString();
        }

        public async Task<User> ActivateEmailCode(ActivateEmailDto activateEmailDto)
        {
            try
            {
                DateTime now = DateTime.UtcNow;
                var userEntity = await GetUserByEmail(activateEmailDto.Email);

                if (userEntity == null)
                {
                    throw new CustomValidationException(CustomValidationCodes.InvalidEmail);
                }

                var emailValidationCodesEntities =
                    await _validationCodesContext
                    .Where(
                            c => c.UserId == userEntity.Id &&
                            c.CodeTypeId == ((int)CodeTypeEnum.Email).ToString() &&
                            c.ExpiresAt > now && 
                            c.ActivatedAt == null
                        )
                    .ToListAsync();

                var isValidationCodeValid = emailValidationCodesEntities.Find(c => BCrypt.Net.BCrypt.Verify(activateEmailDto.Code, c.Code));
                if (isValidationCodeValid == null)
                {
                    throw new CustomValidationException(CustomValidationCodes.InvalidEmailCode);
                }

                isValidationCodeValid.UpdatedAt = now;
                isValidationCodeValid.ActivatedAt = now;
                _validationCodesContext.Update(isValidationCodeValid);

                return userEntity;
            }
            catch (CustomValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<User> VerifyUserPassword(User user)
        {
            try
            {
                var userEntity = await GetUserByEmail(user.Email);

                if (userEntity == null)
                {
                    throw new CustomValidationException(CustomValidationCodes.InvalidEmail);
                }

                if (!BCrypt.Net.BCrypt.EnhancedVerify(user.Password, userEntity.Password))
                {
                    throw new CustomValidationException(CustomValidationCodes.PasswordsDoesntMatch);
                }

                return userEntity;

            } catch (CustomValidationException) {
                throw;
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<bool> ValidateRefreshTokenExist(User user, string refreshToken)
        {
            try
            {
                var validRefreshToken = await _refreshTokensContext.Where(
                    t => t.Token == refreshToken && t.UserId == user.Id && t.ExpiresAt > DateTime.UtcNow
                    )
                    .FirstOrDefaultAsync();

                if (validRefreshToken == null)
                {
                    throw new CustomValidationException(CustomValidationCodes.InvalidRefreshToken);
                }

                validRefreshToken.ExpiresAt = DateTime.UtcNow;
                _refreshTokensContext.Update(validRefreshToken);
                await Save();

                return true;

            }
            catch (CustomValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
