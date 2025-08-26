using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Application.Auth.AuthenticateUser;
using Ambev.DeveloperEvaluation.WebApi.Features.Auth.AuthenticateUserFeature;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Auth.AuthenticateUserFeature;

/// <summary>
/// AutoMapper profile for authentication-related mappings in WebApi layer
/// </summary>
public sealed class AuthMappingProfile : Profile
{
  /// <summary>
  /// Initializes a new instance of the <see cref="AuthMappingProfile"/> class
  /// </summary>
  public AuthMappingProfile()
  {
    // Mapeamento do Request para o Command
    CreateMap<AuthenticateUserRequest, AuthenticateUserCommand>();

    // Mapeamento do Result para a Response
    CreateMap<AuthenticateUserResult, AuthenticateUserResponse>();

    // Mapeamento da entidade User para a Response
    CreateMap<User, AuthenticateUserResponse>()
        .ForMember(dest => dest.Token, opt => opt.Ignore())
        .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));
  }
}
