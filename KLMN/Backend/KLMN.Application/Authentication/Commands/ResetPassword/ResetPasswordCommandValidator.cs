using FluentValidation;

namespace KLMN.Application.Authentication.Commands.ResetPassword;

/// <summary>
/// Reset password doğrulama kurallarını tanımlar.
/// </summary>
public sealed class ResetPasswordCommandValidator
    : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty();

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .MinimumLength(8)
            .MaximumLength(128);

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.NewPassword)
            .WithMessage(
                "Yeni parola ve parola tekrarı aynı olmalıdır.");
    }
}
