using FluentValidation;
namespace KLMN.Application.HcUnusedReasons.Queries.GetHcUnusedReasonForExams;
/// <summary>Sayfa sınırlarını doğrular.</summary>
public sealed class GetHcUnusedReasonForExamsQueryValidator:AbstractValidator<GetHcUnusedReasonForExamsQuery>
{
 public GetHcUnusedReasonForExamsQueryValidator(){RuleFor(x=>x.PageNumber).GreaterThan(0);RuleFor(x=>x.PageSize).InclusiveBetween(1,100);}
}
