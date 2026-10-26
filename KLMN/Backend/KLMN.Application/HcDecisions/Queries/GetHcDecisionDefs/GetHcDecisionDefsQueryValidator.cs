using FluentValidation;
namespace KLMN.Application.HcDecisions.Queries.GetHcDecisionDefs;
/// <summary>Liste API'sinde kabul edilen sayfa sınırları.</summary>
public sealed class GetHcDecisionDefsQueryValidator:AbstractValidator<GetHcDecisionDefsQuery>
{
 public GetHcDecisionDefsQueryValidator(){RuleFor(x=>x.PageNumber).GreaterThan(0);RuleFor(x=>x.PageSize).InclusiveBetween(1,100);}
}
