using FluentValidation;
namespace KLMN.Application.SYYDiagnosisSlices.Queries.GetSYYDiagnosisSliceDefs;
/// <summary>Sayfalama ve arama sınırları.</summary>
public sealed class GetSYYDiagnosisSliceDefsQueryValidator:AbstractValidator<GetSYYDiagnosisSliceDefsQuery>
{
 public GetSYYDiagnosisSliceDefsQueryValidator(){RuleFor(x=>x.PageNumber).GreaterThan(0);
 RuleFor(x=>x.PageSize).InclusiveBetween(1,100);RuleFor(x=>x.Search).MaximumLength(250);}
}
