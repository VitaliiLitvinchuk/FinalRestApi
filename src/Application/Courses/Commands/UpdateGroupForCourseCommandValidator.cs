using FluentValidation;

namespace Application.Courses.Commands;

public class UpdateGroupForCourseCommandValidator : AbstractValidator<UpdateGroupForCourseCommand>
{
    public UpdateGroupForCourseCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.GroupId)
            .NotEmpty();
    }
}
