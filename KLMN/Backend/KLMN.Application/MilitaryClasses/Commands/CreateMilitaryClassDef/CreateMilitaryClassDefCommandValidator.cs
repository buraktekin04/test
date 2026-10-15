using FluentValidation;
using KLMN.Domain.Common;

namespace KLMN.Application.MilitaryClasses.Commands.CreateMilitaryClassDef;

/// <summary>Sınıf alanlarının uzunluk, biçim ve UTC tarih kurallarını doğrular.</summary>
public sealed class CreateMilitaryClassDefCommandValidator : AbstractValidator<CreateMilitaryClassDefCommand>
{
    /// <summary>İşlem öncesinde zorunlu alanları ve geçerlilik dönemini denetler.</summary>
    public CreateMilitaryClassDefCommandValidator()
    {

        RuleFor(x => x.Code).NotEmpty().MaximumLength(100).Matches("^[A-Za-z0-9_-]+$");
        RuleFor(x => x.Name).NotEmpty().MaximumLength(250);
        RuleFor(x => x.ShortName).MaximumLength(100);
        RuleFor(x => x.ExternalCode).MaximumLength(100);
        RuleFor(x => x).Must(x => ValidityPeriod.HasValidBounds(x.ValidFrom, x.ValidTo))
            .WithMessage("Geçerli bitiş başlangıçtan sonra olmalıdır.").WithName("ValidTo");
        RuleFor(x => x.ValidFrom).Must(x => !x.HasValue || x.Value.Kind == DateTimeKind.Utc)
            .WithMessage("Başlangıç tarihi UTC olmalıdır.");
        RuleFor(x => x.ValidTo).Must(x => !x.HasValue || x.Value.Kind == DateTimeKind.Utc)
            .WithMessage("Bitiş tarihi UTC olmalıdır.");
    }
}
