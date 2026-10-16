using FluentValidation;
using KLMN.Domain.Common;

namespace KLMN.Application.AdmissionTypes.Commands.CreateHcAdmissionTypeDef;

/// <summary>Sayısal kod ve geçerlilik dönemi verilerinin doğrulamasıdır.</summary>
public sealed class CreateHcAdmissionTypeDefCommandValidator
    : AbstractValidator<CreateHcAdmissionTypeDefCommand>
{
    /// <summary>Boolean alanlar true/false olarak gönderilir; ayrıca seçim zorunlu değildir.</summary>
    public CreateHcAdmissionTypeDefCommandValidator()
    {
        RuleFor(x => x.Code).GreaterThan(0);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(250);
        RuleFor(x => x).Must(x => ValidityPeriod.HasValidBounds(x.ValidFrom, x.ValidTo))
            .WithMessage("Geçerli bitiş başlangıçtan sonra olmalıdır.").WithName("ValidTo");
        RuleFor(x => x.ValidFrom).Must(x => !x.HasValue || x.Value.Kind == DateTimeKind.Utc)
            .WithMessage("Başlangıç tarihi UTC olmalıdır.");
        RuleFor(x => x.ValidTo).Must(x => !x.HasValue || x.Value.Kind == DateTimeKind.Utc)
            .WithMessage("Bitiş tarihi UTC olmalıdır.");
    }
}
