using Domain.Assignments;
using Domain.Statuses;
using Domain.Users;

namespace Domain.UsersAssignments;

public class UserAssignment(AssignmentId assignmentId, UserId userId, StatusId statusId, DateTime? submittedAt = null, decimal? score = null)
{
    public AssignmentId AssignmentId { get; } = assignmentId;
    public UserId UserId { get; } = userId;
    public StatusId StatusId { get; private set; } = statusId;
    public DateTime? SubmittedAt { get; private set; } = submittedAt;
    public decimal? Score { get; private set; } = score;

    public Assignment? Assignment { get; }
    public Status? Status { get; }
    public User? User { get; }

    public static UserAssignment New(AssignmentId assignmentId, UserId userId, StatusId statusId, DateTime? submittedAt = null, decimal? score = null)
        => new(assignmentId, userId, statusId, submittedAt, score);

    public void UpdateDetails(StatusId statusId, DateTime? submittedAt = null, decimal? score = null)
    {
        StatusId = statusId;
        SubmittedAt = submittedAt;
        Score = score;
    }
}
