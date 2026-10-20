using FluentValidation;

namespace KLMN.Application.RankGroups.Commands.DeleteRankGroupDef;

/// <summary>İsteğin zorunlu alanlarını, UTC dönemini ve xmin parametrelerini denetler.</summary>
public sealed class DeleteRankGroupDefCommandValidator : AbstractValidator<DeleteRankGroupDefCommand>
{
    /// <summary>Rütbe Grubu API sözleşmesine ait validasyon kuralları.</summary>
    public DeleteRankGroupDefCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Version).GreaterThan(0u);

    }
}
