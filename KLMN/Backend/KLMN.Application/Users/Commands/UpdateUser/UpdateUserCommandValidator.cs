using FluentValidation;
using KLMN.Domain.Common;

namespace KLMN.Application.Users.Commands.UpdateUser;

/// <summary>Kullanıcı düzenleme alanlarının doğrulama kurallarıdır.</summary>
public sealed class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    /// <summary>Kimlik, versiyon ve alan uzunluklarını doğrular.</summary>
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Version).GreaterThan(0u);
        RuleFor(x => x.UserName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.PhoneNumber).MaximumLength(30);

        // İsteğe bağlı görev yeri GUID'i verilirse boş/varsayılan GUID kabul edilmez.
        RuleFor(x => x.OrganizationUnitId)
            .Must(id => !id.HasValue || id.Value != Guid.Empty)
            .WithMessage("Geçerli bir görev yeri (organizasyon birimi) seçiniz.");

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
    }
}
