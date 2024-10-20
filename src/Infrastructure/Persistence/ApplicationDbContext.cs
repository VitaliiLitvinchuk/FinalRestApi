using System.Reflection;
using Domain.Assignments;
using Domain.Courses;
using Domain.Groups;
using Domain.Statuses;
using Domain.UserGroupRoles;
using Domain.UserRoles;
using Domain.Users;
using Domain.UsersAssignments;
using Domain.UsersGroups;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<UserGroup> UserGroups => Set<UserGroup>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<UserAssignment> UserAssignments => Set<UserAssignment>();
    public DbSet<UserGroupRole> UserGroupRoles => Set<UserGroupRole>();
    public DbSet<Status> Statuses => Set<Status>();
    public DbSet<Assignment> Assignments => Set<Assignment>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Group> Groups => Set<Group>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
    }
}
