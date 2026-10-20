using FluentValidation;

namespace KLMN.Application.ReportTypes.Commands.ChangeStatusReportTypeDef;

/// <summary>İsteğin zorunlu alanlarını, UTC dönemini ve xmin parametrelerini denetler.</summary>
public sealed class ChangeStatusReportTypeDefCommandValidator : AbstractValidator<ChangeStatusReportTypeDefCommand>
{
    /// <summary>Rapor Türü API sözleşmesine ait validasyon kuralları.</summary>
    public ChangeStatusReportTypeDefCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Version).GreaterThan(0u);

    }
}
