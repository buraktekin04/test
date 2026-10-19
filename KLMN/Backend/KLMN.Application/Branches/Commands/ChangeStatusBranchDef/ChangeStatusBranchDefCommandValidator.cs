using FluentValidation;

namespace KLMN.Application.Branches.Commands.ChangeStatusBranchDef;

/// <summary>İsteğin zorunlu alanlarını, UTC dönemini ve xmin parametrelerini denetler.</summary>
public sealed class ChangeStatusBranchDefCommandValidator : AbstractValidator<ChangeStatusBranchDefCommand>
{
    /// <summary>Branş API sözleşmesine ait validasyon kuralları.</summary>
    public ChangeStatusBranchDefCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Version).GreaterThan(0u);

    }
}
