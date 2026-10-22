using FluentValidation;
namespace KLMN.Application.SYYDiagnosisBranches.Queries.GetSYYDiagnosisBranchDefs;
public sealed class GetSYYDiagnosisBranchDefsQueryValidator:AbstractValidator<GetSYYDiagnosisBranchDefsQuery>
{
 public GetSYYDiagnosisBranchDefsQueryValidator()
 {RuleFor(x=>x.PageNumber).GreaterThan(0);RuleFor(x=>x.PageSize).InclusiveBetween(1,100);
 RuleFor(x=>x.Search).MaximumLength(250);}
}
