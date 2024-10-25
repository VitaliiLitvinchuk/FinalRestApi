using System;
using FluentValidation;

namespace Application.UserGroupRoles.Commands;

public class UpdateUserGroupRoleCommandValidator : AbstractValidator<UpdateUserGroupRoleCommand>
{
    public UpdateUserGroupRoleCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(255);
    }
}
