using FluentValidation;

namespace KLMN.Application.Authentication.Commands.ChangePassword;

/// <summary>ChangePasswordCommand validation kurallarını tanımlar.</summary>
public sealed class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty()
            .MaximumLength(256);

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .MinimumLength(8)
            .MaximumLength(128)
            .Matches("[A-Z]")
            .WithMessage("Yeni parola en az bir büyük harf içermelidir.")
            .Matches("[a-z]")
            .WithMessage("Yeni parola en az bir küçük harf içermelidir.")
            .Matches("[0-9]")
            .WithMessage("Yeni parola en az bir rakam içermelidir.");

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.NewPassword)
            .WithMessage("Yeni parola ve parola tekrarı eşleşmiyor.");

        RuleFor(x => x.NewPassword)
            .NotEqual(x => x.CurrentPassword)
            .WithMessage("Yeni parola mevcut parola ile aynı olamaz.");
    }
}
