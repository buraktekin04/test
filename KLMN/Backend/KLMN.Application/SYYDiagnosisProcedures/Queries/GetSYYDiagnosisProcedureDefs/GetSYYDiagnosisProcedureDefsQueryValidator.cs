using FluentValidation;
namespace KLMN.Application.SYYDiagnosisProcedures.Queries.GetSYYDiagnosisProcedureDefs;
public sealed class GetSYYDiagnosisProcedureDefsQueryValidator:AbstractValidator<GetSYYDiagnosisProcedureDefsQuery>
{
 public GetSYYDiagnosisProcedureDefsQueryValidator()
 {RuleFor(x=>x.PageNumber).GreaterThan(0);RuleFor(x=>x.PageSize).InclusiveBetween(1,100);
 RuleFor(x=>x.Search).MaximumLength(250);}
}
