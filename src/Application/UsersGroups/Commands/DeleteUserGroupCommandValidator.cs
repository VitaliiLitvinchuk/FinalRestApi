using FluentValidation;

namespace Application.UsersGroups.Commands;

public class DeleteUserGroupCommandValidator : AbstractValidator<DeleteUserGroupCommand>
{
    public DeleteUserGroupCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.GroupId)
            .NotEmpty();
    }
}
