using System;
using FluentValidation;

namespace Application.Statuses.Commands;

public class UpdateStatusCommandValidator : AbstractValidator<UpdateStatusCommand>
{
    public UpdateStatusCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(255);
    }
}
