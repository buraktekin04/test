using FluentValidation;

namespace KLMN.Application.Users.Commands.DeleteUser;

/// <summary>Soft delete request'indeki anahtar bilgileri doğrular.</summary>
public sealed class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
{
    /// <summary>Boş ID ve geçersiz xmin versiyonunu reddeder.</summary>
    public DeleteUserCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Version).GreaterThan(0u);
    }
}
