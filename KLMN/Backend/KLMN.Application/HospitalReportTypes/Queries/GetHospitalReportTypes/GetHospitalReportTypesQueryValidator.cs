using FluentValidation;

namespace KLMN.Application.HospitalReportTypes.Queries.GetHospitalReportTypes;

/// <summary>Başvuru grubu/tanım ekranlarıyla aynı sayfa sınırları.</summary>
public sealed class GetHospitalReportTypesQueryValidator
    : AbstractValidator<GetHospitalReportTypesQuery>
{
    /// <summary>Liste/Excel query boyutunu denetler.</summary>
    public GetHospitalReportTypesQueryValidator()
    {
        RuleFor(x => x.Search).MaximumLength(250);
        RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}
