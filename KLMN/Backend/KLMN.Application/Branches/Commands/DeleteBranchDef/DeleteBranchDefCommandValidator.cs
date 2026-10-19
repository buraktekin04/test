using FluentValidation;

namespace KLMN.Application.Branches.Commands.DeleteBranchDef;

/// <summary>İsteğin zorunlu alanlarını, UTC dönemini ve xmin parametrelerini denetler.</summary>
public sealed class DeleteBranchDefCommandValidator : AbstractValidator<DeleteBranchDefCommand>
{
    /// <summary>Branş API sözleşmesine ait validasyon kuralları.</summary>
    public DeleteBranchDefCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Version).GreaterThan(0u);

    }
}
