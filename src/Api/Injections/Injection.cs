using Infrastructure.Persistence;
using Application;

namespace Api.Injections;

public static class Injection
{
    public static IServiceCollection UseInjections(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPersistence(configuration);

        services.AddApplication();

        services.AddRepositories();

        return services;
    }

    private static void AddRepositories(this IServiceCollection services)
    {
    }
}
