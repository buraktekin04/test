using FluentValidation;

namespace KLMN.Application.HospitalReportTypes.Queries.GetHospitalReportTypeGroups;

/// <summary>Mevcut tanım listeleriyle aynı sayfa ve arama sınırları.</summary>
public sealed class GetHospitalReportTypeGroupsQueryValidator
    : AbstractValidator<GetHospitalReportTypeGroupsQuery>
{
    public GetHospitalReportTypeGroupsQueryValidator()
    {
        RuleFor(x => x.Search).MaximumLength(250);
        RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}
