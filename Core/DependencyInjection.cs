using Core.Entities;
using Core.ServiceContracts;
using Core.Services;
using Core.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Core;

public static class DependencyInjection
{
    public static IServiceCollection AddCore(this IServiceCollection services)
    {
        services.AddScoped<IUsersService,UsersService>();
        services.AddScoped<IAuthService,AuthService>();
        services.AddScoped<IPasswordHasher<Users>,PasswordHasher<Users>>();
        services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();
        return services;
    }
}
