using FluentValidation;

namespace KLMN.Application.Ranks.Commands.ChangeStatusRankDef;

/// <summary>İsteğin zorunlu alanlarını, UTC dönemini ve xmin parametrelerini denetler.</summary>
public sealed class ChangeStatusRankDefCommandValidator : AbstractValidator<ChangeStatusRankDefCommand>
{
    /// <summary>Rütbe API sözleşmesine ait validasyon kuralları.</summary>
    public ChangeStatusRankDefCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Version).GreaterThan(0u);

    }
}
