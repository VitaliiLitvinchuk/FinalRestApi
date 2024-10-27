using Domain.Assignments;
using Domain.Statuses;
using Domain.Users;
using Domain.UsersAssignments;
using Infrastructure.Constraints;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class UserAssignmentConfiguration : IEntityTypeConfiguration<UserAssignment>
{
    public void Configure(EntityTypeBuilder<UserAssignment> builder)
    {
        builder.HasKey(x => new { x.AssignmentId, x.UserId });
        builder.Property(x => x.AssignmentId).HasConversion(x => x.Value, x => new AssignmentId(x));
        builder.Property(x => x.UserId).HasConversion(x => x.Value, x => new UserId(x));

        builder.Property(x => x.StatusId).HasConversion(x => x.Value, x => new StatusId(x));

        builder.Property(x => x.SubmittedAt)
            .HasConversion(new DateTimeUtcConverter());

        builder.Property(x => x.Score)
            .HasPrecision(5, 2);

        builder.HasOne(x => x.Assignment)
            .WithMany(x => x.UserAssignments)
            .HasConstraintName("FK_UserAssignment_Assignment_AssignmentId")
            .HasForeignKey(x => x.AssignmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.User)
            .WithMany(x => x.UserAssignments)
            .HasConstraintName("FK_UserAssignment_User_UserId")
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Status)
            .WithMany(x => x.UserAssignments)
            .HasConstraintName("FK_UserAssignment_Status_StatusId")
            .HasForeignKey(x => x.StatusId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
