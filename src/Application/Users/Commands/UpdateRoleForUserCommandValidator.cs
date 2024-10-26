using FluentValidation;

namespace Application.Users.Commands;

public class UpdateRoleForUserCommandValidator : AbstractValidator<UpdateRoleForUserCommand>
{
    public UpdateRoleForUserCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.UserRoleId)
            .NotEmpty();
    }
}
