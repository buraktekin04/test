using FluentValidation;

namespace KLMN.Application.MilitaryUnits.Queries.GetMilitaryUnitDefs;

/// <summary>Arama ve sayfa sınırlarını doğrular.</summary>
public sealed class GetMilitaryUnitDefsQueryValidator : AbstractValidator<GetMilitaryUnitDefsQuery>
{
    public GetMilitaryUnitDefsQueryValidator()
    {
        RuleFor(x => x.Search).MaximumLength(250);
        RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}
