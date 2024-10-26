using Domain.Courses;
using Infrastructure.Constraints;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(x => x.Value, x => new CourseId(x));

        builder.Property(x => x.Name).IsRequired().HasColumnType("varchar(255)");
        builder.Property(x => x.Description).IsRequired().HasColumnType("text");
        builder.Property(x => x.CreatedAt)
            .HasConversion(new DateTimeUtcConverter())
            .HasDefaultValueSql("timezone('utc', now())");

        builder.HasOne(x => x.Group)
            .WithMany(x => x.Courses)
            .HasForeignKey(x => x.GroupId)
            .HasConstraintName("FK_Course_Group_GroupId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.User)
            .WithMany(x => x.Courses)
            .HasForeignKey(x => x.UserId)
            .HasConstraintName("FK_Course_User_UserId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}