using Domain.UserGroupRoles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class UserGroupRoleConfiguration : IEntityTypeConfiguration<UserGroupRole>
{
    public void Configure(EntityTypeBuilder<UserGroupRole> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(x => x.Value, x => new UserGroupRoleId(x));

        builder.Property(x => x.Name).IsRequired().HasColumnType("varchar(255)");

        builder.HasMany(x => x.UserGroups)
            .WithOne(x => x.UserGroupRole)
            .HasForeignKey(x => x.UserGroupRoleId)
            .HasConstraintName("FK_UserGroupRole_UserGroup_UserGroupRoleId")
            .OnDelete(DeleteBehavior.Restrict);
    }
}
