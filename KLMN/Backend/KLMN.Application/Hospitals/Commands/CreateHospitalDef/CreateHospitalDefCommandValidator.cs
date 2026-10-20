using FluentValidation;
using KLMN.Domain.Common;

namespace KLMN.Application.Hospitals.Commands.CreateHospitalDef;

/// <summary>İsteğin zorunlu alanlarını, UTC dönemini ve xmin parametrelerini denetler.</summary>
public sealed class CreateHospitalDefCommandValidator : AbstractValidator<CreateHospitalDefCommand>
{
    /// <summary>Sağlık Tesisi API sözleşmesine ait validasyon kuralları.</summary>
    public CreateHospitalDefCommandValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(100).Matches("^[A-Za-z0-9_-]+$");
        RuleFor(x => x.Name).NotEmpty().MaximumLength(250);
        RuleFor(x => x.CityId).Must(x => !x.HasValue || x.Value != Guid.Empty);
        RuleFor(x => x.RefereeHospitalId).Must(x => !x.HasValue || x.Value != Guid.Empty);
        RuleFor(x => x).Must(x => ValidityPeriod.HasValidBounds(x.ValidFrom, x.ValidTo))
            .WithMessage("Geçerli bitiş başlangıçtan sonra olmalıdır.").WithName("ValidTo");
        RuleFor(x => x.ValidFrom).Must(x => !x.HasValue || x.Value.Kind == DateTimeKind.Utc)
            .WithMessage("Başlangıç tarihi UTC olmalıdır.");
        RuleFor(x => x.ValidTo).Must(x => !x.HasValue || x.Value.Kind == DateTimeKind.Utc)
            .WithMessage("Bitiş tarihi UTC olmalıdır.");
    }
}
