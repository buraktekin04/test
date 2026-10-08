using FluentValidation;

namespace KLMN.Application.Authentication.Commands.Login;

/// <summary>
/// Login request doğrulama kurallarını tanımlar.
/// </summary>
public sealed class LoginCommandValidator
    : AbstractValidator<LoginCommand>
{
    /// <summary>
    /// Login command doğrulama kurallarını oluşturur.
    /// </summary>
    public LoginCommandValidator()
    {
        RuleFor(x => x.Identifier)
            .NotEmpty()
            .MaximumLength(256);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MaximumLength(512);

        RuleFor(x => x.DeviceName)
            .MaximumLength(200);
    }
}
