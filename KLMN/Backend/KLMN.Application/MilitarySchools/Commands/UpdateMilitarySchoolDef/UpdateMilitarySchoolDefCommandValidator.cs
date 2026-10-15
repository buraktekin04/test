using FluentValidation;
using KLMN.Domain.Common;

namespace KLMN.Application.MilitarySchools.Commands.UpdateMilitarySchoolDef;

/// <summary>Okul kodu, iki opsiyonel FK ve UTC tarih aralığını doğrular.</summary>
public sealed class UpdateMilitarySchoolDefCommandValidator : AbstractValidator<UpdateMilitarySchoolDefCommand>
{
    /// <summary>HTTP isteği için veri biçim kurallarını tanımlar.</summary>
    public UpdateMilitarySchoolDefCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Version).GreaterThan(0u);

        RuleFor(x => x.Code).NotEmpty().MaximumLength(100).Matches("^[A-Za-z0-9_-]+$");
        RuleFor(x => x.Name).NotEmpty().MaximumLength(250);
        RuleFor(x => x.CityId).Must(x => !x.HasValue || x.Value != Guid.Empty);
        RuleFor(x => x.ForcesCommandDefId).Must(x => !x.HasValue || x.Value != Guid.Empty);
        RuleFor(x => x).Must(x => ValidityPeriod.HasValidBounds(x.ValidFrom, x.ValidTo))
            .WithMessage("Geçerli bitiş başlangıçtan sonra olmalıdır.").WithName("ValidTo");
        RuleFor(x => x.ValidFrom).Must(x => !x.HasValue || x.Value.Kind == DateTimeKind.Utc)
            .WithMessage("Başlangıç tarihi UTC olmalıdır.");
        RuleFor(x => x.ValidTo).Must(x => !x.HasValue || x.Value.Kind == DateTimeKind.Utc)
            .WithMessage("Bitiş tarihi UTC olmalıdır.");
    }
}
