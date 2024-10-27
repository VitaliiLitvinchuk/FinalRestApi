using FluentValidation;

namespace Application.Assignments.Commands;

public class CreateAssignmentCommandValidator : AbstractValidator<CreateAssignmentCommand>
{
    public CreateAssignmentCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(255);

        RuleFor(x => x.Description)
            .NotEmpty();

        RuleFor(x => x.DueDate)
            .NotEmpty();

        RuleFor(x => x.MaxScore)
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(100);

        RuleFor(x => x.CourseId)
            .NotEmpty();
    }
}
