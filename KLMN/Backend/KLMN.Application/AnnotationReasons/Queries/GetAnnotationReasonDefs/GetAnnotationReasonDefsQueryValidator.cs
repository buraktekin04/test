using FluentValidation;
namespace KLMN.Application.AnnotationReasons.Queries.GetAnnotationReasonDefs;
/// <summary>Sayfalama ve arama sınırları.</summary>
public sealed class GetAnnotationReasonDefsQueryValidator:AbstractValidator<GetAnnotationReasonDefsQuery>
{
 public GetAnnotationReasonDefsQueryValidator(){RuleFor(x=>x.PageNumber).GreaterThan(0);
 RuleFor(x=>x.PageSize).InclusiveBetween(1,100);RuleFor(x=>x.Search).MaximumLength(250);}
}
