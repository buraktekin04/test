using FluentValidation;

namespace KLMN.Application.Organizations.Queries.GetOrganizationUnits;

/// <summary>Arama ve sayfa sınırlarını doğrular.</summary>
public sealed class GetOrganizationUnitsQueryValidator : AbstractValidator<GetOrganizationUnitsQuery>
{
    public GetOrganizationUnitsQueryValidator()
    {
        RuleFor(x => x.Search).MaximumLength(250);
        RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}
