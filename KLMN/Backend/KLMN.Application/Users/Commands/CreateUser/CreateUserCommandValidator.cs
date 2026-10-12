using FluentValidation;

namespace KLMN.Application.Users.Commands.CreateUser;

/// <summary>Kullanıcı ekleme alanları ve ilk parola kurallarını denetler.</summary>
public sealed class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    /// <summary>İsim, benzersiz alan ve parola biçimi kurallarını oluşturur.</summary>
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.UserName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.PhoneNumber).MaximumLength(30);
        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .MaximumLength(128)
            .Matches("[A-Z]").WithMessage("Parola en az bir büyük harf içermelidir.")
            .Matches("[a-z]").WithMessage("Parola en az bir küçük harf içermelidir.")
            .Matches("[0-9]").WithMessage("Parola en az bir rakam içermelidir.");
    }
}
