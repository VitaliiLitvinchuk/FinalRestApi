using FluentValidation;

namespace Application.UserRoles.Commands;

public class DeleteUserRoleCommandValidator : AbstractValidator<DeleteUserRoleCommand>
{
    public DeleteUserRoleCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
