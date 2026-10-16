using FluentValidation;

namespace KLMN.Application.MilitaryOffices.Queries.GetMilitaryOfficeDefs;

/// <summary>Sayfalama, şehir ve arama filtresi sınırlarını denetler.</summary>
public sealed class GetMilitaryOfficeDefsQueryValidator : AbstractValidator<GetMilitaryOfficeDefsQuery>
{
    /// <summary>Toplu listeye gereksiz yük bindiren istekleri reddeder.</summary>
    public GetMilitaryOfficeDefsQueryValidator()
    {
        RuleFor(x => x.Search).MaximumLength(250);
        RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        RuleFor(x => x.CityId).Must(x => !x.HasValue || x.Value != Guid.Empty);
    }
}
