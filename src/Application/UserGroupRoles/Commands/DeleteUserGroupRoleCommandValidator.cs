using FluentValidation;

namespace Application.UserGroupRoles.Commands;

public class DeleteUserGroupRoleCommandValidator : AbstractValidator<DeleteUserGroupRoleCommand>
{
    public DeleteUserGroupRoleCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
