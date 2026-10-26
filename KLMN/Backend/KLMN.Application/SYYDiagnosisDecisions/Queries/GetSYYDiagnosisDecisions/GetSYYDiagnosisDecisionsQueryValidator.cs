using FluentValidation;
namespace KLMN.Application.SYYDiagnosisDecisions.Queries.GetSYYDiagnosisDecisions;
/// <summary>Kullanıcı filtrelerinin ve sayfalama sınırının doğrulanması.</summary>
public sealed class GetSYYDiagnosisDecisionsQueryValidator:AbstractValidator<GetSYYDiagnosisDecisionsQuery>
{
 public GetSYYDiagnosisDecisionsQueryValidator(){
  RuleFor(x=>x.PageNumber).GreaterThan(0);
  RuleFor(x=>x.PageSize).InclusiveBetween(1,100);
  RuleFor(x=>x.Search).MaximumLength(300);
 }
}
