using FluentValidation;
namespace KLMN.Application.Cities.Queries.GetCityDefs;
public sealed class GetCityDefsQueryValidator : AbstractValidator<GetCityDefsQuery>
{
    public GetCityDefsQueryValidator()
    {
        RuleFor(x => x.Search).MaximumLength(250);
        RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}
