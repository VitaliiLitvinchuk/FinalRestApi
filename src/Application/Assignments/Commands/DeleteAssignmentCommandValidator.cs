using FluentValidation;

namespace Application.Assignments.Commands;

public class DeleteAssignmentCommandValidator : AbstractValidator<DeleteAssignmentCommand>
{
    public DeleteAssignmentCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
