using FluentValidation;

namespace Application.Assignments.Commands;

public class UpdateCourseForAssignmentCommandValidator : AbstractValidator<UpdateCourseForAssignmentCommand>
{
    public UpdateCourseForAssignmentCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.CourseId)
            .NotEmpty();
    }
}
