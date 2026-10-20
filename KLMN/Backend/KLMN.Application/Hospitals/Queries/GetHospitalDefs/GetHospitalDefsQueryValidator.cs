using FluentValidation;

namespace KLMN.Application.Hospitals.Queries.GetHospitalDefs;

/// <summary>Başvuru grubu/tanım ekranlarıyla aynı sayfa sınırları.</summary>
public sealed class GetHospitalDefsQueryValidator
    : AbstractValidator<GetHospitalDefsQuery>
{
    /// <summary>Liste/Excel query boyutunu denetler.</summary>
    public GetHospitalDefsQueryValidator()
    {
        RuleFor(x => x.Search).MaximumLength(250);
        RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}
