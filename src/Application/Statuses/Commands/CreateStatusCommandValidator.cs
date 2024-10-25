using FluentValidation;

namespace Application.Statuses.Commands;

public class CreateStatusCommandValidator : AbstractValidator<CreateStatusCommand>
{
    public CreateStatusCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(255);
    }
}
