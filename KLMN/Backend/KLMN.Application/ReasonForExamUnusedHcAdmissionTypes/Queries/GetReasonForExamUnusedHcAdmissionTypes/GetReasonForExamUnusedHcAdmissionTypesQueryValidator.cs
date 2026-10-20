using FluentValidation;
namespace KLMN.Application.ReasonForExamUnusedHcAdmissionTypes.Queries.GetReasonForExamUnusedHcAdmissionTypes;
/// <summary>Liste ve Excel sorgularının maliyet sınırları.</summary>
public sealed class GetReasonForExamUnusedHcAdmissionTypesQueryValidator:AbstractValidator<GetReasonForExamUnusedHcAdmissionTypesQuery>
{
 public GetReasonForExamUnusedHcAdmissionTypesQueryValidator(){RuleFor(x=>x.Search).MaximumLength(250);RuleFor(x=>x.PageNumber).GreaterThan(0);
 RuleFor(x=>x.PageSize).InclusiveBetween(1,100);}
}
