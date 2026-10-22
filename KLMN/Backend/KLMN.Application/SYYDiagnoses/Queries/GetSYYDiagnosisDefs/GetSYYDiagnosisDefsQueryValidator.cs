using FluentValidation;
namespace KLMN.Application.SYYDiagnoses.Queries.GetSYYDiagnosisDefs;
/// <summary>Sayfalama ve arama sınırları.</summary>
public sealed class GetSYYDiagnosisDefsQueryValidator:AbstractValidator<GetSYYDiagnosisDefsQuery>
{
 public GetSYYDiagnosisDefsQueryValidator(){RuleFor(x=>x.PageNumber).GreaterThan(0);
 RuleFor(x=>x.PageSize).InclusiveBetween(1,100);RuleFor(x=>x.Search).MaximumLength(250);}
}
