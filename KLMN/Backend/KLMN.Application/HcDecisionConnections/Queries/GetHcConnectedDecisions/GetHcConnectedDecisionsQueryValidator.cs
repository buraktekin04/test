using FluentValidation;
namespace KLMN.Application.HcDecisionConnections.Queries.GetHcConnectedDecisions;
/// <summary>Sayfa sınırlarını doğrular.</summary>
public sealed class GetHcConnectedDecisionsQueryValidator:AbstractValidator<GetHcConnectedDecisionsQuery>
{
 public GetHcConnectedDecisionsQueryValidator(){RuleFor(x=>x.PageNumber).GreaterThan(0);RuleFor(x=>x.PageSize).InclusiveBetween(1,100);}
}
