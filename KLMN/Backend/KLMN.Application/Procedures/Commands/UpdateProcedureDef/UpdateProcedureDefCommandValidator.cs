using FluentValidation;
using KLMN.Domain.Common;

namespace KLMN.Application.Procedures.Commands.UpdateProcedureDef;

/// <summary>İsteğin zorunlu alanlarını, UTC dönemini ve xmin parametrelerini denetler.</summary>
public sealed class UpdateProcedureDefCommandValidator : AbstractValidator<UpdateProcedureDefCommand>
{
    /// <summary>Tetkik API sözleşmesine ait validasyon kuralları.</summary>
    public UpdateProcedureDefCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Version).GreaterThan(0u);
        RuleFor(x => x.ProcedureCode).NotEmpty().MaximumLength(100).Matches("^[A-Za-z0-9_-]+$");
        RuleFor(x => x.Name).NotEmpty().MaximumLength(250);
        RuleFor(x => x.ProcedureType).GreaterThanOrEqualTo(0);
        RuleFor(x => x).Must(x => ValidityPeriod.HasValidBounds(x.StartDate, x.EndDate))
            .WithMessage("Geçerli bitiş başlangıçtan sonra olmalıdır.").WithName("EndDate");
        RuleFor(x => x.StartDate).Must(x => !x.HasValue || x.Value.Kind == DateTimeKind.Utc)
            .WithMessage("Başlangıç tarihi UTC olmalıdır.");
        RuleFor(x => x.EndDate).Must(x => !x.HasValue || x.Value.Kind == DateTimeKind.Utc)
            .WithMessage("Bitiş tarihi UTC olmalıdır.");
    }
}
