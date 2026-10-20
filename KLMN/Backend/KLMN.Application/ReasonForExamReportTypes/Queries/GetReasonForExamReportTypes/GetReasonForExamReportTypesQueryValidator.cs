using FluentValidation;
namespace KLMN.Application.ReasonForExamReportTypes.Queries.GetReasonForExamReportTypes;
/// <summary>Liste ve Excel sorgularının maliyet sınırları.</summary>
public sealed class GetReasonForExamReportTypesQueryValidator:AbstractValidator<GetReasonForExamReportTypesQuery>
{
 public GetReasonForExamReportTypesQueryValidator(){RuleFor(x=>x.Search).MaximumLength(250);RuleFor(x=>x.PageNumber).GreaterThan(0);
 RuleFor(x=>x.PageSize).InclusiveBetween(1,100);}
}
