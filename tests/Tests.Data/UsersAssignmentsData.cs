using Domain.Assignments;
using Domain.Statuses;
using Domain.Users;
using Domain.UsersAssignments;

namespace Tests.Data;

public class UsersAssignmentsData
{
    public static UserAssignment MainAssignment(AssignmentId assignmentId, UserId userId, StatusId statusId, DateTime? submittedAt = null, decimal? score = null)
        => UserAssignment.New(assignmentId, userId, statusId, submittedAt, score);
}
