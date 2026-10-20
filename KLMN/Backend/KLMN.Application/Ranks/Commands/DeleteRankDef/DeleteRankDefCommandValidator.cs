using FluentValidation;

namespace KLMN.Application.Ranks.Commands.DeleteRankDef;

/// <summary>İsteğin zorunlu alanlarını, UTC dönemini ve xmin parametrelerini denetler.</summary>
public sealed class DeleteRankDefCommandValidator : AbstractValidator<DeleteRankDefCommand>
{
    /// <summary>Rütbe API sözleşmesine ait validasyon kuralları.</summary>
    public DeleteRankDefCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Version).GreaterThan(0u);

    }
}
