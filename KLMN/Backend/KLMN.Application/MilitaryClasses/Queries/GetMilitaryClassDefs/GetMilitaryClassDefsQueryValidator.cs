using FluentValidation;

namespace KLMN.Application.MilitaryClasses.Queries.GetMilitaryClassDefs;

/// <summary>Sınıf listelemesinde arama ve sayfalama sınırlarını doğrular.</summary>
public sealed class GetMilitaryClassDefsQueryValidator : AbstractValidator<GetMilitaryClassDefsQuery>
{
    /// <summary>Güvenli sayfa uzunluğu ve arama metni sınırlarını belirler.</summary>
    public GetMilitaryClassDefsQueryValidator()
    {
        RuleFor(x => x.Search).MaximumLength(250);
        RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}
