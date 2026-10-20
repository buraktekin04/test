using FluentValidation;
using KLMN.Domain.Common;

namespace KLMN.Application.HospitalReportTypes.Commands.UpdateHospitalReportType;

/// <summary>İsteğin zorunlu alanlarını, UTC dönemini ve xmin parametrelerini denetler.</summary>
public sealed class UpdateHospitalReportTypeCommandValidator : AbstractValidator<UpdateHospitalReportTypeCommand>
{
    /// <summary>Sağlık Tesisi Rapor Türü API sözleşmesine ait validasyon kuralları.</summary>
    public UpdateHospitalReportTypeCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Version).GreaterThan(0u);
        RuleFor(x => x.HospitalDefId).NotEmpty();
        RuleFor(x => x.ReportTypeDefId).NotEmpty();
    }
}
