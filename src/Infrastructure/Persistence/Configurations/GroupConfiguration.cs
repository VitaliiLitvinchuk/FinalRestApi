using Domain.Groups;
using Infrastructure.Constraints;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class GroupConfiguration : IEntityTypeConfiguration<Group>
{
    public void Configure(EntityTypeBuilder<Group> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(x => x.Value, x => new GroupId(x));

        builder.Property(x => x.Name).IsRequired().HasColumnType("varchar(255)");
        builder.Property(x => x.Description).IsRequired().HasColumnType("text");
        builder.Property(x => x.CreatedAt)
            .HasConversion(new DateTimeUtcConverter())
            .HasDefaultValueSql("timezone('utc', now())");

        builder.HasMany(x => x.UserGroups)
            .WithOne(x => x.Group)
            .HasForeignKey(x => x.GroupId)
            .HasConstraintName("FK_Group_UserGroup_GroupId")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Courses)
            .WithOne(x => x.Group)
            .HasForeignKey(x => x.GroupId)
            .HasConstraintName("FK_Group_Course_GroupId")
            .OnDelete(DeleteBehavior.Restrict);
    }
}
