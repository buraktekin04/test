using FluentValidation;

namespace KLMN.Application.MilitaryClassGroupRelations.Queries.GetMilitaryClassGroupRelations;

/// <summary>İlişki listesi sayfalama ve GUID filtrelerini doğrular.</summary>
public sealed class GetMilitaryClassGroupRelationsQueryValidator : AbstractValidator<GetMilitaryClassGroupRelationsQuery>
{
    /// <summary>100 kayıt üst sınırı ve geçersiz GUID koruması.</summary>
    public GetMilitaryClassGroupRelationsQueryValidator()
    {
        RuleFor(x => x.Search).MaximumLength(250);
        RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        RuleFor(x => x.ForcesCommandId).Must(x => !x.HasValue || x.Value != Guid.Empty);
        RuleFor(x => x.MilitaryClassId).Must(x => !x.HasValue || x.Value != Guid.Empty);
        RuleFor(x => x.MilitaryClassGroupId).Must(x => !x.HasValue || x.Value != Guid.Empty);
    }
}
