using FluentValidation;

namespace Application.Courses.Commands;

public class CreateCourseCommandValidator : AbstractValidator<CreateCourseCommand>
{
    public CreateCourseCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(255);

        RuleFor(x => x.Description)
            .NotEmpty();

        RuleFor(x => x.GroupId)
            .NotEmpty();
    }
}
