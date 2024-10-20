using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Infrastructure.Persistence;

public static class ConfigurePersistence
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var dataSourceDataBuilder = new NpgsqlDataSourceBuilder(configuration.GetConnectionString("DefaultConnection"));

        dataSourceDataBuilder.EnableDynamicJson();

        var dataSource = dataSourceDataBuilder.Build();

        services
            .AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(dataSource,
                    builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName))
                .UseSnakeCaseNamingConvention()
                .ConfigureWarnings(x => x.Ignore(RelationalEventId.MultipleCollectionIncludeWarning)));

        services.AddScoped<ApplicationDbContextInitializer>();

        return services;
    }
}
