using FluentValidation;

namespace KLMN.Application.Authentication.Commands.ForgotPassword;

/// <summary>ForgotPasswordCommand validation kurallarını tanımlar.</summary>
public sealed class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
{
    /// <summary>
    /// Komutun çalıştırılmasından önce giriş alanlarının doğrulama kurallarını tanımlar.
    /// </summary>
    public ForgotPasswordCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(256);
    }
}
