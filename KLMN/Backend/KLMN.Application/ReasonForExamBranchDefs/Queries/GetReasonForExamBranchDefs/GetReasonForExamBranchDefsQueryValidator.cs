using FluentValidation;
namespace KLMN.Application.ReasonForExamBranchDefs.Queries.GetReasonForExamBranchDefs;
/// <summary>Liste ve Excel sorgularının maliyet sınırları.</summary>
public sealed class GetReasonForExamBranchDefsQueryValidator:AbstractValidator<GetReasonForExamBranchDefsQuery>
{
 public GetReasonForExamBranchDefsQueryValidator(){RuleFor(x=>x.Search).MaximumLength(250);RuleFor(x=>x.PageNumber).GreaterThan(0);
 RuleFor(x=>x.PageSize).InclusiveBetween(1,100);}
}
