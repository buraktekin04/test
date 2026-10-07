using FluentValidation;

namespace KLMN.Application.Authentication.Commands.Login;

/// <summary>
/// Login request doğrulama kurallarını tanımlar.
/// </summary>
public sealed class LoginCommandValidator
    : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.UserNameOrEmail)
            .NotEmpty()
            .MaximumLength(256);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MaximumLength(512);
    }
}
