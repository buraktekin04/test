using FluentValidation;
namespace KLMN.Application.ReasonForExamProcedureDefs.Queries.GetReasonForExamProcedureDefs;
/// <summary>Liste ve Excel sorgularının maliyet sınırları.</summary>
public sealed class GetReasonForExamProcedureDefsQueryValidator:AbstractValidator<GetReasonForExamProcedureDefsQuery>
{
 public GetReasonForExamProcedureDefsQueryValidator(){RuleFor(x=>x.Search).MaximumLength(250);RuleFor(x=>x.PageNumber).GreaterThan(0);
 RuleFor(x=>x.PageSize).InclusiveBetween(1,100);}
}
