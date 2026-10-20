using FluentValidation;
using KLMN.Domain.Common;

namespace KLMN.Application.Ranks.Commands.CreateRankDef;

/// <summary>İsteğin zorunlu alanlarını, UTC dönemini ve xmin parametrelerini denetler.</summary>
public sealed class CreateRankDefCommandValidator : AbstractValidator<CreateRankDefCommand>
{
    /// <summary>Rütbe API sözleşmesine ait validasyon kuralları.</summary>
    public CreateRankDefCommandValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(100).Matches("^[A-Za-z0-9_-]+$");
        RuleFor(x => x.Name).NotEmpty().MaximumLength(250);
        RuleFor(x => x.ShortName).MaximumLength(100);
        RuleFor(x => x.RankGroupId).Must(id => !id.HasValue || id.Value != Guid.Empty);
        RuleFor(x => x).Must(x => ValidityPeriod.HasValidBounds(x.ValidFrom, x.ValidTo))
            .WithMessage("Geçerli bitiş başlangıçtan sonra olmalıdır.").WithName("ValidTo");
        RuleFor(x => x.ValidFrom).Must(x => !x.HasValue || x.Value.Kind == DateTimeKind.Utc)
            .WithMessage("Başlangıç tarihi UTC olmalıdır.");
        RuleFor(x => x.ValidTo).Must(x => !x.HasValue || x.Value.Kind == DateTimeKind.Utc)
            .WithMessage("Bitiş tarihi UTC olmalıdır.");
    }
}
