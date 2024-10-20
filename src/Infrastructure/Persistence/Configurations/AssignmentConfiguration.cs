using Domain.Assignments;
using Infrastructure.Constraints;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class AssignmentConfiguration : IEntityTypeConfiguration<Assignment>
{
    public void Configure(EntityTypeBuilder<Assignment> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(x => x.Id).HasConversion(x => x.Value, x => new AssignmentId(x));

        builder.Property(x => x.Title).IsRequired().HasColumnType("varchar(255)");
        builder.Property(x => x.Description).IsRequired().HasColumnType("text");
        builder.Property(x => x.MaxScore)
            .HasPrecision(5, 2)
            .HasDefaultValue(0);

        builder.Property(x => x.DueDate)
            .IsRequired()
            .HasConversion(new DateTimeUtcConverter());

        builder.Property(x => x.CreatedAt)
            .HasConversion(new DateTimeUtcConverter())
            .HasDefaultValueSql("timezone('utc', now())");

        builder.HasMany(x => x.UserAssignments)
            .WithOne(x => x.Assignment)
            .HasForeignKey(x => x.AssignmentId)
            .HasConstraintName("FK_Assignment_UserAssignment_AssignmentId")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Course)
            .WithMany(x => x.Assignments)
            .HasForeignKey(x => x.CourseId)
            .HasConstraintName("FK_Assignment_Course_CourseId")
            .OnDelete(DeleteBehavior.Restrict);
    }
}
