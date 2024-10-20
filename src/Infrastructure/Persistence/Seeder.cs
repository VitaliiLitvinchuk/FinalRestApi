using Domain.Assignments;
using Domain.Courses;
using Domain.Groups;
using Domain.Statuses;
using Domain.UserGroupRoles;
using Domain.UserRoles;
using Domain.Users;
using Domain.UsersAssignments;
using Domain.UsersGroups;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.Persistence;

public static class Seeder
{
    public static async Task SeedAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var env = scope.ServiceProvider.GetRequiredService<IHostEnvironment>();

        await SeedUserRoles(context.UserRoles);
        await SeedUserGroupRoles(context.UserGroupRoles);
        await SeedStatuses(context.Statuses);
        await SeedGroups(context.Groups);

        await context.SaveChangesAsync();

        if (env.IsDevelopment())
        {
            await SeedUsers(context.Users, context.UserRoles);
            await context.SaveChangesAsync();

            await SeedUserGroups(context.UserGroups, context.Users, context.Groups, context.UserGroupRoles);
            await context.SaveChangesAsync();

            await SeedCourses(context.Courses, context.Users, context.Groups);
            await context.SaveChangesAsync();

            await SeedAssignments(context.Assignments, context.Courses);
            await context.SaveChangesAsync();

            await SeedUserAssignments(context.UserAssignments, context.Users, context.Assignments, context.Statuses);
            await context.SaveChangesAsync();
        }
    }

    public static async Task SeedUserRoles(DbSet<UserRole> userRoles)
    {
        if (!userRoles.Any())
        {
            IEnumerable<UserRole> roles = [
                UserRole.New(UserRoleId.New(), "Admin"),
                UserRole.New(UserRoleId.New(), "Moderator"),
                UserRole.New(UserRoleId.New(), "User")
            ];

            await userRoles.AddRangeAsync(roles);
        }
    }

    public static async Task SeedUserGroupRoles(DbSet<UserGroupRole> userGroupRoles)
    {
        if (!userGroupRoles.Any())
        {
            IEnumerable<UserGroupRole> roles = [
                UserGroupRole.New(UserGroupRoleId.New(), "Moderator"),
                UserGroupRole.New(UserGroupRoleId.New(), "Member"),
            ];

            await userGroupRoles.AddRangeAsync(roles);
        }
    }

    public static async Task SeedStatuses(DbSet<Status> statuses)
    {
        if (!statuses.Any())
        {
            IEnumerable<Status> status = [
                Status.New(StatusId.New(), "Not Started"),
                Status.New(StatusId.New(), "In Progress"),
                Status.New(StatusId.New(), "Done")
            ];

            await statuses.AddRangeAsync(status);
        }
    }

    public static async Task SeedGroups(DbSet<Group> groups)
    {
        if (!groups.Any())
        {
            IEnumerable<Group> groupList = [
                Group.New(GroupId.New(), "Group1", "Desc1", DateTime.UtcNow),
                Group.New(GroupId.New(), "Group2", "Desc2", DateTime.UtcNow),
                Group.New(GroupId.New(), "Group3", "Desc3", DateTime.UtcNow),
                Group.New(GroupId.New(), "Group4", "Desc4", DateTime.UtcNow),
                Group.New(GroupId.New(), "Group5", "Desc5", DateTime.UtcNow)
            ];

            await groups.AddRangeAsync(groupList);
        }
    }

    public static async Task SeedUsers(DbSet<User> users, DbSet<UserRole> userRoles)
    {
        if (!users.Any())
        {
            IEnumerable<User> userList = [
                User.New(UserId.New(), "John", "Doe", "john.doe@example.com", "12345", "no_image.png", userRoles.Single(x => x.Name == "Admin").Id, DateTime.UtcNow),
            ];

            await users.AddRangeAsync(userList);
        }
    }

    public static async Task SeedUserGroups(DbSet<UserGroup> userGroups, DbSet<User> users, DbSet<Group> groups, DbSet<UserGroupRole> userGroupRoles)
    {
        if (!userGroups.Any())
        {
            var roles = userGroupRoles.Single(y => y.Name == "Moderator");
            IEnumerable<UserGroup> userGroupList = groups
                .Select(x => UserGroup.New(users.Single(y => y.Email == "john.doe@example.com").Id, x.Id, roles.Id, DateTime.UtcNow));

            await userGroups.AddRangeAsync(userGroupList);
        }
    }

    public static async Task SeedAssignments(DbSet<Assignment> assignments, DbSet<Course> courses)
    {
        if (!assignments.Any())
        {
            IEnumerable<Assignment> assignmentList = courses.ToList()
                .Select((x, i) => Assignment.New(AssignmentId.New(), x.Id, $"Assignment {i + 1}", $"Description {i + 1}", DateTime.UtcNow.AddDays(7 * (i + 1)), 100m, DateTime.UtcNow));

            await assignments.AddRangeAsync(assignmentList);
        }
    }

    public static async Task SeedUserAssignments(DbSet<UserAssignment> userAssignments, DbSet<User> users, DbSet<Assignment> assignments, DbSet<Status> statuses)
    {
        if (!userAssignments.Any())
        {
            var user = users.Single(x => x.Email == "john.doe@example.com");
            var statuss = statuses.ToList();
            IEnumerable<UserAssignment> userAssignmentList = assignments.ToList()
                .Select((x, i) => UserAssignment.New(x.Id, user.Id, statuss[i % 3].Id));

            await userAssignments.AddRangeAsync(userAssignmentList);
        }
    }
    public static async Task SeedCourses(DbSet<Course> courses, DbSet<User> users, DbSet<Group> groups)
    {
        if (!courses.Any())
        {
            var user = users.Single(x => x.Email == "john.doe@example.com");
            IEnumerable<Course> courseList = groups.ToList()
                .Select((x, i) => Course.New(CourseId.New(), $"Course {i + 1}", $"Description {i + 1}", user.Id, x.Id, DateTime.UtcNow));

            await courses.AddRangeAsync(courseList);
        }
    }
}
