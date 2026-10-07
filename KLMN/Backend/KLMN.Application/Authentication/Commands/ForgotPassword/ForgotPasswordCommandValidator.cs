using FluentValidation;

namespace KLMN.Application.Authentication.Commands.ForgotPassword;

/// <summary>
/// Forgot password doğrulama kurallarını tanımlar.
/// </summary>
public sealed class ForgotPasswordCommandValidator
    : AbstractValidator<ForgotPasswordCommand>
{
    public ForgotPasswordCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(256);
    }
}
