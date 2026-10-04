using AutoMapper;
using Core.DTOs;
using Core.Entities;

namespace Core.Mapping;

public class ApplicationUserMappingProfile : Profile
{
  public ApplicationUserMappingProfile()
  {
    CreateMap<Users, AuthenticationResponse>()
      .ForMember(dest => dest.UserID, opt => opt.MapFrom(src => src.UserID))
      .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
      .ForMember(dest => dest.PersonName, opt => opt.MapFrom(src => src.Name))
      .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
      .ForMember(dest => dest.Success, opt => opt.Ignore())
      .ForMember(dest => dest.AccsesToken, opt => opt.Ignore())
      .ForMember(des=>des.RefreshToken,op=>op.MapFrom(src=>src.RefreshToken))
      .ForMember(des=>des.RefreshTokenExpiryTime,op=>op.MapFrom(src=>src.RefreshTokenExpiryTime));
  }
}
