using FluentValidation;

namespace KLMN.Application.Branches.Queries.GetBranchDefs;

/// <summary>Başvuru grubu/tanım ekranlarıyla aynı sayfa sınırları.</summary>
public sealed class GetBranchDefsQueryValidator
    : AbstractValidator<GetBranchDefsQuery>
{
    /// <summary>Liste/Excel query boyutunu denetler.</summary>
    public GetBranchDefsQueryValidator()
    {
        RuleFor(x => x.Search).MaximumLength(250);
        RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}
