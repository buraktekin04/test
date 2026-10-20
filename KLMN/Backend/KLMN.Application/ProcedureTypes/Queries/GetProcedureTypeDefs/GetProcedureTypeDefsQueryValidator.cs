using FluentValidation;
namespace KLMN.Application.ProcedureTypes.Queries.GetProcedureTypeDefs;
/// <summary>Tetkik türleri liste sorgusunun sınırları.</summary>
public sealed class GetProcedureTypeDefsQueryValidator:AbstractValidator<GetProcedureTypeDefsQuery>
{
  public GetProcedureTypeDefsQueryValidator()
  {
    RuleFor(x=>x.PageNumber).GreaterThan(0);
    RuleFor(x=>x.PageSize).InclusiveBetween(1,100);
    RuleFor(x=>x.Search).MaximumLength(250);
  }
}
