using AutoMapper;
using Core.DTOs;
using Core.Entities;

namespace Core.Mapping;

public class RegisterRequestMappingProfile:Profile
{
    public RegisterRequestMappingProfile()
    {
        CreateMap<RegisterRequest,Users>()
        .ForMember(des=>des.Email,op=>op.MapFrom(src=>src.Email))
        .ForMember(des=>des.Name,op=>op.MapFrom(src=>src.PersonName))
        .ForMember(des=>des.Gender,op=>op.MapFrom(src=>src.Gender.ToString()))
        .ForMember(des=>des.PasswordHash,op=>op.Ignore());
    }
}
