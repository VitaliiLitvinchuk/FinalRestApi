using FluentValidation;

namespace Application.UserRoles.Commands;

public class CreateUserRoleCommandValidator : AbstractValidator<CreateUserRoleCommand>
{
    public CreateUserRoleCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(5)
            .MaximumLength(255);
    }
}
