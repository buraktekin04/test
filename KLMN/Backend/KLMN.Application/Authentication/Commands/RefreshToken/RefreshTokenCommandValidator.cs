using FluentValidation;

namespace KLMN.Application.Authentication.Commands.RefreshToken;

/// <summary>RefreshTokenCommand doğrulama kurallarını tanımlar.</summary>
public sealed class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .WithMessage("Refresh token bilgisi bulunamadı.");
    }
}
