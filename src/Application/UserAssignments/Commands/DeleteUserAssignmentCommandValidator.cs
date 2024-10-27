using FluentValidation;

namespace Application.UserAssignments.Commands;

public class DeleteUserAssignmentCommandValidator : AbstractValidator<DeleteUserAssignmentCommand>
{
    public DeleteUserAssignmentCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.AssignmentId)
            .NotEmpty();
    }
}
