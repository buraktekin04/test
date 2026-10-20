using FluentValidation;

namespace KLMN.Application.RankGroups.Queries.GetRankGroupDefs;

/// <summary>Başvuru grubu/tanım ekranlarıyla aynı sayfa sınırları.</summary>
public sealed class GetRankGroupDefsQueryValidator
    : AbstractValidator<GetRankGroupDefsQuery>
{
    /// <summary>Liste/Excel query boyutunu denetler.</summary>
    public GetRankGroupDefsQueryValidator()
    {
        RuleFor(x => x.Search).MaximumLength(250);
        RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}
