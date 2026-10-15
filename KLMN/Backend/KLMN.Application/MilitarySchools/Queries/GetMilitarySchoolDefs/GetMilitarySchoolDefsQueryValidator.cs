using FluentValidation;

namespace KLMN.Application.MilitarySchools.Queries.GetMilitarySchoolDefs;

/// <summary>Sunucu tarafında sayfalama sınırlarını ve filtreleri doğrular.</summary>
public sealed class GetMilitarySchoolDefsQueryValidator : AbstractValidator<GetMilitarySchoolDefsQuery>
{
    /// <summary>Sayfalama ve geçersiz filtre GUID'lerini sınırlar.</summary>
    public GetMilitarySchoolDefsQueryValidator()
    {
        RuleFor(x => x.Search).MaximumLength(250);
        RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        RuleFor(x => x.CityId).Must(x => !x.HasValue || x.Value != Guid.Empty);
        RuleFor(x => x.ForcesCommandDefId).Must(x => !x.HasValue || x.Value != Guid.Empty);
    }
}
