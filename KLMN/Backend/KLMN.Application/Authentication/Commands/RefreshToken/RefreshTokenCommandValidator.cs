using FluentValidation;

namespace KLMN.Application.Authentication.Commands.RefreshToken;

/// <summary>RefreshTokenCommand doğrulama kurallarını tanımlar.</summary>
public sealed class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    /// <summary>
    /// Komutun çalıştırılmasından önce giriş alanlarının doğrulama kurallarını tanımlar.
    /// </summary>
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .WithMessage("Refresh token bilgisi bulunamadı.");
    }
}
