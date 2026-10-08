using FluentValidation;

namespace KLMN.Application.Authentication.Commands.Login;

/// <summary>LoginCommand validation kurallarını tanımlar.</summary>
public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Identifier)
            .NotEmpty()
            .WithMessage("Kullanıcı adı veya e-posta adresi zorunludur.")
            .MaximumLength(256);

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Parola zorunludur.")
            .MaximumLength(256);

        RuleFor(x => x.DeviceName)
            .MaximumLength(200);
    }
}
