using AgroShop.Application.Extensions;
using AgroShop.Domain.Errors;
using AgroShop.Domain.Users;
using FluentValidation;

namespace AgroShop.Application.Users.LoginUser;

public sealed class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
{
    public LoginUserCommandValidator()
    {
        ClassLevelCascadeMode = CascadeMode.Continue;
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Phone).MustBeValueObject(Phone.Create);
        RuleFor(x => x.Password).NotEmpty().WithError(DomainErrors.Users.PasswordIsRequired());
    }
}
