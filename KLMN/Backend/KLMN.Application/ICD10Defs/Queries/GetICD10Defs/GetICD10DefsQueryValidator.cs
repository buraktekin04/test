using FluentValidation;
namespace KLMN.Application.ICD10Defs.Queries.GetICD10Defs;
public sealed class GetICD10DefsQueryValidator:AbstractValidator<GetICD10DefsQuery>
{
 public GetICD10DefsQueryValidator()
 {
  RuleFor(x=>x.PageNumber).GreaterThan(0);
  RuleFor(x=>x.PageSize).InclusiveBetween(1,100);
  RuleFor(x=>x.Search).MaximumLength(250);
 }
}
