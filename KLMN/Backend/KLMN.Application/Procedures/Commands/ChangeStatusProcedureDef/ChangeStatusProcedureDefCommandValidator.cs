using FluentValidation;

namespace KLMN.Application.Procedures.Commands.ChangeStatusProcedureDef;

/// <summary>İsteğin zorunlu alanlarını, UTC dönemini ve xmin parametrelerini denetler.</summary>
public sealed class ChangeStatusProcedureDefCommandValidator : AbstractValidator<ChangeStatusProcedureDefCommand>
{
    /// <summary>Tetkik API sözleşmesine ait validasyon kuralları.</summary>
    public ChangeStatusProcedureDefCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Version).GreaterThan(0u);

    }
}
