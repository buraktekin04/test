using FluentValidation;
using KLMN.Domain.Common;

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

        // İsteğe bağlı görev yeri GUID'i verilirse boş/varsayılan GUID kabul edilmez.
        RuleFor(x => x.MilitaryUnitDefId)
            .Must(id => !id.HasValue || id.Value != Guid.Empty)
            .WithMessage("Geçerli bir görev yeri (birlik) seçiniz.");

        // Tarih aralığı açık uçlu olabilir ancak ters/eşit aralığa izin verilmez.
        RuleFor(x => x)
            .Must(x => ValidityPeriod.HasValidBounds(x.ValidFrom, x.ValidTo))
            .WithMessage("Bitiş tarihi başlangıç tarihinden sonra olmalıdır.")
            .WithName("ValidTo");

        // PostgreSQL timestamptz alanlarına yalnızca UTC değerleri yazılır.
        RuleFor(x => x.ValidFrom)
            .Must(x => !x.HasValue || x.Value.Kind == DateTimeKind.Utc)
            .WithMessage("Başlangıç tarihi UTC formatında gönderilmelidir.");
        RuleFor(x => x.ValidTo)
            .Must(x => !x.HasValue || x.Value.Kind == DateTimeKind.Utc)
            .WithMessage("Bitiş tarihi UTC formatında gönderilmelidir.");
        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .MaximumLength(128)
            .Matches("[A-Z]").WithMessage("Parola en az bir büyük harf içermelidir.")
            .Matches("[a-z]").WithMessage("Parola en az bir küçük harf içermelidir.")
            .Matches("[0-9]").WithMessage("Parola en az bir rakam içermelidir.");
    }
}
