using FluentValidation;
namespace KLMN.Application.ICD10Diagnostics.Queries.GetICD10Diagnostics;
public sealed class GetICD10DiagnosticsQueryValidator:AbstractValidator<GetICD10DiagnosticsQuery>
{
 public GetICD10DiagnosticsQueryValidator()
 {
  RuleFor(x=>x.PageNumber).GreaterThan(0);
  RuleFor(x=>x.PageSize).InclusiveBetween(1,100);
  RuleFor(x=>x.Search).MaximumLength(250);
 }
}
