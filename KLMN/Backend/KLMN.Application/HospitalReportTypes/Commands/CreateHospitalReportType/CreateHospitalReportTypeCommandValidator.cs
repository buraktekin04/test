using FluentValidation;
using KLMN.Domain.Common;

namespace KLMN.Application.HospitalReportTypes.Commands.CreateHospitalReportType;

/// <summary>İsteğin zorunlu alanlarını, UTC dönemini ve xmin parametrelerini denetler.</summary>
public sealed class CreateHospitalReportTypeCommandValidator : AbstractValidator<CreateHospitalReportTypeCommand>
{
    /// <summary>Sağlık Tesisi Rapor Türü API sözleşmesine ait validasyon kuralları.</summary>
    public CreateHospitalReportTypeCommandValidator()
    {
        RuleFor(x => x.HospitalDefId).NotEmpty();
        RuleFor(x => x.ReportTypeDefId).NotEmpty();
    }
}
