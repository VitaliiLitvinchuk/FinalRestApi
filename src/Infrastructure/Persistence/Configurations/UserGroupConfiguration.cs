using Domain.Groups;
using Domain.Users;
using Domain.UsersGroups;
using Infrastructure.Constraints;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class UserGroupConfiguration : IEntityTypeConfiguration<UserGroup>
{
    public void Configure(EntityTypeBuilder<UserGroup> builder)
    {
        builder.HasKey(x => new { x.GroupId, x.UserId });
        builder.Property(x => x.GroupId).HasConversion(x => x.Value, x => new GroupId(x));
        builder.Property(x => x.UserId).HasConversion(x => x.Value, x => new UserId(x));

        builder.Property(x => x.JoinedAt)
            .HasConversion(new DateTimeUtcConverter())
            .HasDefaultValueSql("timezone('utc', now())");

        builder.HasOne(x => x.Group)
            .WithMany(x => x.UserGroups)
            .HasForeignKey(x => x.GroupId)
            .HasConstraintName("FK_UserGroup_Group_GroupId")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.User)
            .WithMany(x => x.UserGroups)
            .HasForeignKey(x => x.UserId)
            .HasConstraintName("FK_UserGroup_User_UserId")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.UserGroupRole)
            .WithMany(x => x.UserGroups)
            .HasForeignKey(x => x.UserGroupRoleId)
            .HasConstraintName("FK_UserGroup_UserGroupRole_UserGroupRoleId")
            .OnDelete(DeleteBehavior.Restrict);
    }
}
