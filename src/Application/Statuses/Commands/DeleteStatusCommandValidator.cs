using FluentValidation;

namespace Application.Statuses.Commands;

public class DeleteStatusCommandValidator : AbstractValidator<DeleteStatusCommand>
{
    public DeleteStatusCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}
