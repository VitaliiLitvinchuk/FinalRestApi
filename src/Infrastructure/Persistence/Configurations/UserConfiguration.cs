using Domain.Users;
using Infrastructure.Constraints;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(x => x.Value, x => new UserId(x));

        builder.Property(x => x.FirstName).IsRequired().HasColumnType("varchar(255)");
        builder.Property(x => x.LastName).IsRequired().HasColumnType("varchar(255)");
        builder.Property(x => x.Email).IsRequired().HasColumnType("varchar(255)");
        builder.Property(x => x.GoogleId).IsRequired().HasColumnType("varchar(255)");
        builder.Property(x => x.AvatarUrl).IsRequired().HasColumnType("varchar(255)");

        builder.Property(x => x.CreatedAt)
            .HasConversion(new DateTimeUtcConverter())
            .HasDefaultValueSql("timezone('utc', now())");

        builder.HasOne(x => x.UserRole)
            .WithMany(x => x.Users)
            .HasForeignKey(x => x.UserRoleId)
            .HasConstraintName("FK_User_UserRole_UserRoleId")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.UserAssignments)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .HasConstraintName("FK_User_UserAssignment_UserId")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Courses)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .HasConstraintName("FK_User_Course_UserId")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.UserGroups)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .HasConstraintName("FK_User_UserGroup_UserId")
            .OnDelete(DeleteBehavior.Restrict);
    }
}
