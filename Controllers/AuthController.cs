using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServerlessLogin.Dtos;
using ServerlessLogin.Dtos.Auth;
using ServerlessLogin.Dtos.General;
using ServerlessLogin.Filters;
using ServerlessLogin.Filters.ExceptionFilters.Auth;
using ServerlessLogin.Interfaces;
using ServerlessLogin.Models;

namespace ServerlessLogin.Controllers
{
    [Route("/auth")]
    [ApiController]
    [TypeFilter(typeof(AuthExceptionFilter))]
    public class AuthController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailRepository _emailRepository;
        private readonly IJwtManagerRepository _jwtManagerRepository;
        private readonly IMapper _mapper;

        public AuthController(
            IUserRepository userRepository,
            IEmailRepository emailRepository,
            IJwtManagerRepository jwtManagerRepository,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _emailRepository = emailRepository;
            _jwtManagerRepository = jwtManagerRepository;
            _mapper = mapper;
        }

        [HttpPost("sign-up", Name = "Signup")]
        [ProducesResponseType(201, Type = typeof(APIResponseDto<SignupResponseDto>))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Signup(
                [FromBody]
                SignupDto payload
            )
        {
            var response = new SignupResponseDto()
            {
                EmailSent = true,
            };

            try
            {
                var userMap = _mapper.Map<SignupDto, User>(payload);
                var userCreated = await _userRepository.CreateUser(userMap);

                string emailCode = _userRepository.CreateRandomCode();

                ValidationCode validationCode = await _userRepository
                    .CreateValidationCode(
                        userMap,
                        (int)CodeTypeEnum.Email,
                        emailCode
                 );

                await _userRepository.Save();
                await _emailRepository.SendValidationCodeEmail(userMap, emailCode);
                
                return StatusCode(201, response);
            }
            catch (Exception ex)
            {
                //response.Ok = false;
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        [HttpPost("activate-email", Name = "ActivateEmailCode")]
        [ProducesResponseType(200, Type = typeof(APIResponseDto<ActivateEmailResponseDto>))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> ActivateEmailCode(
                [FromBody]
                ActivateEmailDto payload
            )
        {
            try
            {
                var userEntity = await _userRepository.ActivateEmailCode(payload);
                var tokens = _jwtManagerRepository.GenerateJWT(userEntity.Email);

                await _userRepository.SaveRefreshToken(userEntity, tokens);

                var mappedResponse = _mapper.Map<Tokens, ActivateEmailResponseDto>(tokens);

                await _userRepository.Save();
                return Ok(mappedResponse);

            }
            catch
            {
                throw;
            }
        }

        [HttpPost("log-in", Name = "Login")]
        [ProducesResponseType(200, Type = typeof(APIResponseDto<LoginResponseDto>))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Login(
               [FromBody]
                LoginDto payload
           )
        {
            try
            {
                var userMapped = _mapper.Map<LoginDto, User>(payload);
                var userEntity = await _userRepository.VerifyUserPassword(userMapped);
                var tokens = _jwtManagerRepository.GenerateJWT(userEntity.Email);

                await _userRepository.SaveRefreshToken(userEntity, tokens);
                var mappedResponse = _mapper.Map<Tokens, LoginResponseDto>(tokens);

                await _userRepository.Save();

                return Ok(mappedResponse);
            } catch
            {
                throw;
            }
        }

        [HttpGet("validate-jwt", Name = "ValidateJWT")]
        [Authorize]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public IActionResult ValidateJWT()
        {
            var response = new HealthCheckResponseDto();
            return Ok(response);
        }

        [HttpPost("refresh-token", Name = "RefreshToken")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> RefreshToken(
               [FromBody]
                RefreshTokenDto payload
           )
        {
            try
            {

                string refreshToken = payload.RefreshToken;

                var principal = _jwtManagerRepository.GetPrincipalFromExpiredToken(refreshToken);
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                var username = principal.Identity.Name;
#pragma warning restore CS8602 // Dereference of a possibly null reference.

                var user = await _userRepository.GetUserByEmail(username);
                var isValidUser = await _userRepository.IsValidUser(user.Id);

                if (!isValidUser)
                {
                    throw new CustomValidationException(CustomValidationCodes.UserNotFound);
                }

                await _userRepository.ValidateRefreshTokenExist(user, refreshToken);

                var tokens = _jwtManagerRepository.GenerateJWT(username);
                var mappedResponse = _mapper.Map<Tokens, RefreshTokenResponseDto>(tokens);

                return Ok(mappedResponse);
            }
            catch
            {
                throw;
            }
           
        }
    }
}
