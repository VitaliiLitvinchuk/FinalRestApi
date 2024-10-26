using FluentValidation;

namespace Application.UsersGroups.Commands;

public class CreateUserGroupCommandValidator : AbstractValidator<CreateUserGroupCommand>
{
    public CreateUserGroupCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.GroupId)
            .NotEmpty();
    }
}
