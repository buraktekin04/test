using FluentValidation;

namespace KLMN.Application.RankGroups.Commands.ChangeStatusRankGroupDef;

/// <summary>İsteğin zorunlu alanlarını, UTC dönemini ve xmin parametrelerini denetler.</summary>
public sealed class ChangeStatusRankGroupDefCommandValidator : AbstractValidator<ChangeStatusRankGroupDefCommand>
{
    /// <summary>Rütbe Grubu API sözleşmesine ait validasyon kuralları.</summary>
    public ChangeStatusRankGroupDefCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Version).GreaterThan(0u);

    }
}
