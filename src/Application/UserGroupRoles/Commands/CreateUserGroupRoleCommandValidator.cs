using FluentValidation;

namespace Application.UserGroupRoles.Commands;

public class CreateUserGroupRoleCommandValidator : AbstractValidator<CreateUserGroupRoleCommand>
{
    public CreateUserGroupRoleCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(1)
            .MaximumLength(255);
    }
}
