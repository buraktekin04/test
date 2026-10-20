using FluentValidation;

namespace KLMN.Application.Hospitals.Commands.ChangeStatusHospitalDef;

/// <summary>İsteğin zorunlu alanlarını, UTC dönemini ve xmin parametrelerini denetler.</summary>
public sealed class ChangeStatusHospitalDefCommandValidator : AbstractValidator<ChangeStatusHospitalDefCommand>
{
    /// <summary>Sağlık Tesisi API sözleşmesine ait validasyon kuralları.</summary>
    public ChangeStatusHospitalDefCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Version).GreaterThan(0u);

    }
}
