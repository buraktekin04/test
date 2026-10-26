using FluentValidation;
namespace KLMN.Application.HcUnusedDecisions.Queries.GetHcUnusedDecisions;
/// <summary>Sayfa sınırlarını doğrular.</summary>
public sealed class GetHcUnusedDecisionsQueryValidator:AbstractValidator<GetHcUnusedDecisionsQuery>
{
 public GetHcUnusedDecisionsQueryValidator(){RuleFor(x=>x.PageNumber).GreaterThan(0);RuleFor(x=>x.PageSize).InclusiveBetween(1,100);}
}
