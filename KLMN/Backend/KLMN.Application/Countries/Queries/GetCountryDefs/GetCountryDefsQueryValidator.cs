using FluentValidation;
namespace KLMN.Application.Countries.Queries.GetCountryDefs;
public sealed class GetCountryDefsQueryValidator : AbstractValidator<GetCountryDefsQuery>
{
    public GetCountryDefsQueryValidator()
    {
        RuleFor(x => x.Search).MaximumLength(250);
        RuleFor(x => x.ShortCode).MaximumLength(10);
        RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}
