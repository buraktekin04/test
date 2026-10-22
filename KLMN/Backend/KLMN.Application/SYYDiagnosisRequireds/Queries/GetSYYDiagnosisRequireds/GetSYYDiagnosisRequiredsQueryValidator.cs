using FluentValidation;
namespace KLMN.Application.SYYDiagnosisRequireds.Queries.GetSYYDiagnosisRequireds;
public sealed class GetSYYDiagnosisRequiredsQueryValidator:AbstractValidator<GetSYYDiagnosisRequiredsQuery>
{
 public GetSYYDiagnosisRequiredsQueryValidator(){RuleFor(x=>x.PageNumber).GreaterThan(0);
 RuleFor(x=>x.PageSize).InclusiveBetween(1,100);RuleFor(x=>x.Search).MaximumLength(250);}
}
