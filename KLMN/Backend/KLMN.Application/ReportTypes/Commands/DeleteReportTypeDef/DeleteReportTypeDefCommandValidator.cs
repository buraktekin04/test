using FluentValidation;

namespace KLMN.Application.ReportTypes.Commands.DeleteReportTypeDef;

/// <summary>İsteğin zorunlu alanlarını, UTC dönemini ve xmin parametrelerini denetler.</summary>
public sealed class DeleteReportTypeDefCommandValidator : AbstractValidator<DeleteReportTypeDefCommand>
{
    /// <summary>Rapor Türü API sözleşmesine ait validasyon kuralları.</summary>
    public DeleteReportTypeDefCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Version).GreaterThan(0u);

    }
}
