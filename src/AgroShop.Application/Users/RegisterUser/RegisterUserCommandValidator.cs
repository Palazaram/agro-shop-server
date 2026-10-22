using AgroShop.Application.Extensions;
using AgroShop.Domain.Errors;
using AgroShop.Domain.Users;
using FluentValidation;

namespace AgroShop.Application.Users.RegisterUser;

public sealed class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        ClassLevelCascadeMode = CascadeMode.Continue;
        RuleLevelCascadeMode = CascadeMode.Stop;       

        RuleFor(x => x.FirstName).MustBeValueObject(PersonName.Create);
        RuleFor(x => x.LastName).MustBeValueObject(PersonName.Create);
        RuleFor(x => x.Patronymic).MustBeValueObject(PersonName.Create);
        RuleFor(x => x.Email).MustBeValueObject(Email.Create);
        RuleFor(x => x.Phone).MustBeValueObject(Phone.Create);
        RuleFor(x => x.Password).MustBeValueObject(Password.Create);
        RuleFor(x => x.AcceptTerms).Equal(true).WithError(DomainErrors.Users.TermsNotAccepted());
    }
}