using Core.RepositoryContracts;
using Infrastructure.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IUsersRepository,UsersRepository>();
        services.AddScoped<IRolesRepository,RolesRepository>();
        services.AddDbContext<AppdbContext>();
        return services;
    }
}
