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
            .MinimumLength(5)
            .MaximumLength(255);
    }
}
