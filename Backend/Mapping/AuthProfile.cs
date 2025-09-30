using AutoMapper;
using Backend.Domain.DTOs.Auth;
using Backend.Domain.Models;

namespace Backend.Mapping
{
    public class AuthProfile : Profile
    {
        public AuthProfile()
        {
            CreateMap<AppUser, AuthResDTO>()
                .ForMember(dest => dest.Token, opt => opt.Ignore())
                .ForMember(dest => dest.ExpireAt, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.Ignore());
        }
    }
}
