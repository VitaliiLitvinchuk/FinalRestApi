using FluentValidation;

namespace Application.UsersGroups.Commands;

public class UpdateRoleForUserGroupCommandValidator : AbstractValidator<UpdateRoleForUserGroupCommand>
{
    public UpdateRoleForUserGroupCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.GroupId)
            .NotEmpty();

        RuleFor(x => x.UserGroupRoleId)
            .NotEmpty();
    }
}
