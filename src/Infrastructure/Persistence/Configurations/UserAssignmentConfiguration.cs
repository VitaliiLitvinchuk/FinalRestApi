using Domain.Assignments;
using Domain.Users;
using Domain.UsersAssignments;
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

        builder.HasOne(x => x.Assignment)
            .WithMany(x => x.UserAssignments)
            .HasConstraintName("FK_UserAssignment_Assignment_AssignmentId")
            .HasForeignKey(x => x.AssignmentId);

        builder.HasOne(x => x.User)
            .WithMany(x => x.UserAssignments)
            .HasConstraintName("FK_UserAssignment_User_UserId")
            .HasForeignKey(x => x.UserId);

        builder.HasOne(x => x.Status)
            .WithMany(x => x.UserAssignments)
            .HasConstraintName("FK_UserAssignment_Status_StatusId")
            .HasForeignKey(x => x.StatusId);
    }
}
