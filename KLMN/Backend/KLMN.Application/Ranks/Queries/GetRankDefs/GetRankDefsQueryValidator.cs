using FluentValidation;

namespace KLMN.Application.Ranks.Queries.GetRankDefs;

/// <summary>Başvuru grubu/tanım ekranlarıyla aynı sayfa sınırları.</summary>
public sealed class GetRankDefsQueryValidator
    : AbstractValidator<GetRankDefsQuery>
{
    /// <summary>Liste/Excel query boyutunu denetler.</summary>
    public GetRankDefsQueryValidator()
    {
        RuleFor(x => x.Search).MaximumLength(250);
        RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}
