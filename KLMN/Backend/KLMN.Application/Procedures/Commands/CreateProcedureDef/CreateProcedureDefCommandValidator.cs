using FluentValidation;
using KLMN.Domain.Common;

namespace KLMN.Application.Procedures.Commands.CreateProcedureDef;

/// <summary>İsteğin zorunlu alanlarını, UTC dönemini ve xmin parametrelerini denetler.</summary>
public sealed class CreateProcedureDefCommandValidator : AbstractValidator<CreateProcedureDefCommand>
{
    /// <summary>Tetkik API sözleşmesine ait validasyon kuralları.</summary>
    public CreateProcedureDefCommandValidator()
    {
        RuleFor(x => x.ProcedureCode).NotEmpty().MaximumLength(100).Matches("^[A-Za-z0-9_-]+$");
        RuleFor(x => x.Name).NotEmpty().MaximumLength(250);
        RuleFor(x => x.ProcedureType).GreaterThanOrEqualTo(0);
        RuleFor(x => x).Must(x => ValidityPeriod.HasValidBounds(x.ValidFrom, x.ValidTo))
            .WithMessage("Geçerli bitiş başlangıçtan sonra olmalıdır.").WithName("ValidTo");
        RuleFor(x => x.ValidFrom).Must(x => !x.HasValue || x.Value.Kind == DateTimeKind.Utc)
            .WithMessage("Başlangıç tarihi UTC olmalıdır.");
        RuleFor(x => x.ValidTo).Must(x => !x.HasValue || x.Value.Kind == DateTimeKind.Utc)
            .WithMessage("Bitiş tarihi UTC olmalıdır.");
    }
}
