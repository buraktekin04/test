using FluentValidation;

namespace KLMN.Application.Users.Commands.ChangeUserStatus;

/// <summary>Durum güncellemede istenen kayıt kimliği ve versiyonunu doğrular.</summary>
public sealed class ChangeUserStatusCommandValidator : AbstractValidator<ChangeUserStatusCommand>
{
    /// <summary>Boş kimlik veya geçersiz concurrency versiyonunu reddeder.</summary>
    public ChangeUserStatusCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Version).GreaterThan(0u);
    }
}
