using FluentValidation;

namespace KLMN.Application.HospitalReportTypes.Commands.DeleteHospitalReportType;

/// <summary>İsteğin zorunlu alanlarını, UTC dönemini ve xmin parametrelerini denetler.</summary>
public sealed class DeleteHospitalReportTypeCommandValidator : AbstractValidator<DeleteHospitalReportTypeCommand>
{
    /// <summary>Sağlık Tesisi Rapor Türü API sözleşmesine ait validasyon kuralları.</summary>
    public DeleteHospitalReportTypeCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Version).GreaterThan(0u);

    }
}
