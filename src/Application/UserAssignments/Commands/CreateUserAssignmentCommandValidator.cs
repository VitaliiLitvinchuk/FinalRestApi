using FluentValidation;

namespace Application.UserAssignments.Commands;

public class CreateUserAssignmentCommandValidator : AbstractValidator<CreateUserAssignmentCommand>
{
    public CreateUserAssignmentCommandValidator()
    {
        RuleFor(x => x.AssignmentId)
            .NotEmpty();

        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}
