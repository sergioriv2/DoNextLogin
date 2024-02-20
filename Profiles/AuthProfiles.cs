using AutoMapper;
using ServerlessLogin.Dtos.Auth;
using ServerlessLogin.Models;

namespace ServerlessLogin.Profiles
{
    public class AuthProfiles : Profile
    {
        public AuthProfiles()
        {
            CreateMap<SignupDto, User>();
            CreateMap<LoginDto, User>();
            CreateMap<Tokens, ActivateEmailResponseDto>();
            CreateMap<Tokens, LoginResponseDto>();
            CreateMap<Tokens, RefreshTokenResponseDto>();
        }
    }
}
