using FluentValidation;

namespace KLMN.Application.Authentication.Commands.ChangePassword;

/// <summary>
/// Change password doğrulama kurallarını tanımlar.
/// </summary>
public sealed class ChangePasswordCommandValidator
    : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(x => x.CurrentPassword)
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
