using FluentValidation;
namespace KLMN.Application.PlaneTypes.Queries.GetPlaneTypeDefs;
/// <summary>Sayfalama ve arama sınırları.</summary>
public sealed class GetPlaneTypeDefsQueryValidator:AbstractValidator<GetPlaneTypeDefsQuery>
{
 public GetPlaneTypeDefsQueryValidator(){RuleFor(x=>x.PageNumber).GreaterThan(0);
 RuleFor(x=>x.PageSize).InclusiveBetween(1,100);RuleFor(x=>x.Search).MaximumLength(250);}
}
