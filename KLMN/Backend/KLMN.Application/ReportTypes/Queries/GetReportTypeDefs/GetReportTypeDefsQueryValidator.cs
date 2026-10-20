using FluentValidation;

namespace KLMN.Application.ReportTypes.Queries.GetReportTypeDefs;

/// <summary>Başvuru grubu/tanım ekranlarıyla aynı sayfa sınırları.</summary>
public sealed class GetReportTypeDefsQueryValidator
    : AbstractValidator<GetReportTypeDefsQuery>
{
    /// <summary>Liste/Excel query boyutunu denetler.</summary>
    public GetReportTypeDefsQueryValidator()
    {
        RuleFor(x => x.Search).MaximumLength(250);
        RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}
