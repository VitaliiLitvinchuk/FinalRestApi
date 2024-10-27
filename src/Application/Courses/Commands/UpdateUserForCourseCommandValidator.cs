using FluentValidation;

namespace Application.Courses.Commands;

public class UpdateUserForCourseCommandValidator : AbstractValidator<UpdateUserForCourseCommand>
{
    public UpdateUserForCourseCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}
