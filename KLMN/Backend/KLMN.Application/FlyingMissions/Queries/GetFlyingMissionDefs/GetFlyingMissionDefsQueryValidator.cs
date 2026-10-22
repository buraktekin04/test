using FluentValidation;
namespace KLMN.Application.FlyingMissions.Queries.GetFlyingMissionDefs;
/// <summary>Sayfalama ve arama sınırları.</summary>
public sealed class GetFlyingMissionDefsQueryValidator:AbstractValidator<GetFlyingMissionDefsQuery>
{
 public GetFlyingMissionDefsQueryValidator(){RuleFor(x=>x.PageNumber).GreaterThan(0);
 RuleFor(x=>x.PageSize).InclusiveBetween(1,100);RuleFor(x=>x.Search).MaximumLength(250);}
}
