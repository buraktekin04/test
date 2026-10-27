using FluentValidation;
namespace KLMN.Application.EquivalentProcedures.Queries.GetEquivalentProcedureOptions;
public sealed class GetEquivalentProcedureOptionsQueryValidator:AbstractValidator<GetEquivalentProcedureOptionsQuery>
{
 public GetEquivalentProcedureOptionsQueryValidator(){
  RuleFor(x=>x.Search).MaximumLength(200);
 }
}
