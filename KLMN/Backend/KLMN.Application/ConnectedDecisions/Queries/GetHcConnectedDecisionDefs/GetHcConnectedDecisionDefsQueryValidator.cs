using FluentValidation;

namespace KLMN.Application.ConnectedDecisions.Queries.GetHcConnectedDecisionDefs;

/// <summary>Başvuru grubu/tanım ekranlarıyla aynı sayfa sınırları.</summary>
public sealed class GetHcConnectedDecisionDefsQueryValidator
    : AbstractValidator<GetHcConnectedDecisionDefsQuery>
{
    /// <summary>Liste/Excel query boyutunu denetler.</summary>
    public GetHcConnectedDecisionDefsQueryValidator()
    {
        RuleFor(x => x.Search).MaximumLength(250);
        RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}
