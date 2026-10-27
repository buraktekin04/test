using FluentValidation;
namespace KLMN.Application.EquivalentProcedures.Queries.GetEquivalentProcedures;
public sealed class GetEquivalentProceduresQueryValidator:AbstractValidator<GetEquivalentProceduresQuery>
{
 public GetEquivalentProceduresQueryValidator(){
  RuleFor(x=>x.Search).MaximumLength(200);
  RuleFor(x=>x.PageNumber).GreaterThan(0);
  RuleFor(x=>x.PageSize).InclusiveBetween(1,100);
 }
}
