using FluentValidation;
namespace KLMN.Application.ReasonForExaminations.Queries.GetReasonForExaminationDefs;
/// <summary>Sunucu sayfalama ve arama sınırları.</summary>
public sealed class GetReasonForExaminationDefsQueryValidator:AbstractValidator<GetReasonForExaminationDefsQuery>
{
 public GetReasonForExaminationDefsQueryValidator(){
 RuleFor(x=>x.Search).MaximumLength(250);RuleFor(x=>x.PageNumber).GreaterThan(0);
 RuleFor(x=>x.PageSize).InclusiveBetween(1,100); }
}
