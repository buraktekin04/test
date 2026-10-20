using FluentValidation;

namespace KLMN.Application.Procedures.Commands.DeleteProcedureDef;

/// <summary>İsteğin zorunlu alanlarını, UTC dönemini ve xmin parametrelerini denetler.</summary>
public sealed class DeleteProcedureDefCommandValidator : AbstractValidator<DeleteProcedureDefCommand>
{
    /// <summary>Tetkik API sözleşmesine ait validasyon kuralları.</summary>
    public DeleteProcedureDefCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Version).GreaterThan(0u);

    }
}
