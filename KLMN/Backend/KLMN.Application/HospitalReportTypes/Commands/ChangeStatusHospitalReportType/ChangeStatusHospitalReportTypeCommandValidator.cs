using FluentValidation;

namespace KLMN.Application.HospitalReportTypes.Commands.ChangeStatusHospitalReportType;

/// <summary>İsteğin zorunlu alanlarını, UTC dönemini ve xmin parametrelerini denetler.</summary>
public sealed class ChangeStatusHospitalReportTypeCommandValidator : AbstractValidator<ChangeStatusHospitalReportTypeCommand>
{
    /// <summary>Sağlık Tesisi Rapor Türü API sözleşmesine ait validasyon kuralları.</summary>
    public ChangeStatusHospitalReportTypeCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Version).GreaterThan(0u);

    }
}
