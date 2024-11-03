using FluentValidation;

namespace Application.UserRoles.Commands;

public class UpdateUserRoleCommandValidator : AbstractValidator<UpdateUserRoleCommand>
{

    public UpdateUserRoleCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(1)
            .MaximumLength(255);
    }
}
