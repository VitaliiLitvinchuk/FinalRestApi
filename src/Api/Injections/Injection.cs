using Infrastructure.Persistence;
using Application;
using Application.Common.Interfaces.Repositories;
using Infrastructure.Persistence.Repositories;
using Application.Common.Interfaces.Queries;

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
        services.AddScoped<AssignmentRepository>();
        services.AddScoped<IAssignmentRepository>(provider => provider.GetRequiredService<AssignmentRepository>());
        services.AddScoped<IAssignmentQueries>(provider => provider.GetRequiredService<AssignmentRepository>());

        services.AddScoped<CourseRepository>();
        services.AddScoped<ICourseRepository>(provider => provider.GetRequiredService<CourseRepository>());
        services.AddScoped<ICourseQueries>(provider => provider.GetRequiredService<CourseRepository>());

        services.AddScoped<GroupRepository>();
        services.AddScoped<IGroupRepository>(provider => provider.GetRequiredService<GroupRepository>());
        services.AddScoped<IGroupQueries>(provider => provider.GetRequiredService<GroupRepository>());

        services.AddScoped<StatusRepository>();
        services.AddScoped<IStatusRepository>(provider => provider.GetRequiredService<StatusRepository>());
        services.AddScoped<IStatusQueries>(provider => provider.GetRequiredService<StatusRepository>());

        services.AddScoped<UserAssignmentRepository>();
        services.AddScoped<IUserAssignmentRepository>(provider => provider.GetRequiredService<UserAssignmentRepository>());
        services.AddScoped<IUserAssignmentQueries>(provider => provider.GetRequiredService<UserAssignmentRepository>());

        services.AddScoped<UserRepository>();
        services.AddScoped<IUserRepository>(provider => provider.GetRequiredService<UserRepository>());
        services.AddScoped<IUserQueries>(provider => provider.GetRequiredService<UserRepository>());

        services.AddScoped<UserGroupRepository>();
        services.AddScoped<IUserGroupRepository>(provider => provider.GetRequiredService<UserGroupRepository>());
        services.AddScoped<IUserGroupQueries>(provider => provider.GetRequiredService<UserGroupRepository>());

        services.AddScoped<UserGroupRoleRepository>();
        services.AddScoped<IUserGroupRoleRepository>(provider => provider.GetRequiredService<UserGroupRoleRepository>());
        services.AddScoped<IUserGroupRoleQueries>(provider => provider.GetRequiredService<UserGroupRoleRepository>());

        services.AddScoped<UserRoleRepository>();
        services.AddScoped<IUserRoleRepository>(provider => provider.GetRequiredService<UserRoleRepository>());
        services.AddScoped<IUserRoleQueries>(provider => provider.GetRequiredService<UserRoleRepository>());
    }
}
