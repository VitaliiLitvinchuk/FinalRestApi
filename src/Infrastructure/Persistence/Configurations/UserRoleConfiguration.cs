using Domain.UserRoles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(x => x.Value, x => new UserRoleId(x));

        builder.Property(x => x.Name).IsRequired().HasColumnType("varchar(255)");

        builder.HasMany(x => x.Users)
            .WithOne(x => x.UserRole)
            .HasForeignKey(x => x.UserRoleId)
            .HasConstraintName("FK_UserRole_User_UserRoleId")
            .OnDelete(DeleteBehavior.Restrict);
    }
}
