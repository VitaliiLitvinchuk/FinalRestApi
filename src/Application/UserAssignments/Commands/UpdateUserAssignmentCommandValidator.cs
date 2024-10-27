using FluentValidation;

namespace Application.UserAssignments.Commands;

public class UpdateUserAssignmentCommandValidator : AbstractValidator<UpdateUserAssignmentCommand>
{
    public UpdateUserAssignmentCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.AssignmentId)
            .NotEmpty();

        RuleFor(x => x.StatusId)
            .NotEmpty();
    }
}
