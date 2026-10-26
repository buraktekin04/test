using FluentValidation;
namespace KLMN.Application.SYYRules.Queries.GetSYYRules;
/// <summary>Sorgu sınırları; tüm veriyi tek sayfada çekmeye izin verilmez.</summary>
public sealed class GetSYYRulesQueryValidator:AbstractValidator<GetSYYRulesQuery>
{
 public GetSYYRulesQueryValidator(){
  RuleFor(x=>x.PageNumber).GreaterThan(0);
  RuleFor(x=>x.PageSize).InclusiveBetween(1,100);
  RuleFor(x=>x.Search).MaximumLength(300);
 }
}
