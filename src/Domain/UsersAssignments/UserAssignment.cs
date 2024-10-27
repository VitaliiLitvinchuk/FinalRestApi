using Domain.Assignments;
using Domain.Statuses;
using Domain.Users;

namespace Domain.UsersAssignments;

public class UserAssignment(AssignmentId assignmentId, UserId userId, StatusId statusId)
{
    public AssignmentId AssignmentId { get; } = assignmentId;
    public UserId UserId { get; } = userId;
    public StatusId StatusId { get; private set; } = statusId;
    public DateTime? SubmittedAt { get; private set; } = null;
    public decimal? Score { get; private set; } = null;

    public Assignment? Assignment { get; }
    public Status? Status { get; }
    public User? User { get; }

    public static UserAssignment New(AssignmentId assignmentId, UserId userId, StatusId statusId)
        => new(assignmentId, userId, statusId);

    public void UpdateDetails(StatusId statusId, DateTime? submittedAt = null, decimal? score = null)
    {
        StatusId = statusId;
        SubmittedAt = submittedAt;
        Score = score;
    }
}
