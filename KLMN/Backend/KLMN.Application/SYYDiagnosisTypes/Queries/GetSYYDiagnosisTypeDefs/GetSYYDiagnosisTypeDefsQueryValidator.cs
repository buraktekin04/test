using FluentValidation;
namespace KLMN.Application.SYYDiagnosisTypes.Queries.GetSYYDiagnosisTypeDefs;
/// <summary>Sayfalama ve arama sınırları.</summary>
public sealed class GetSYYDiagnosisTypeDefsQueryValidator:AbstractValidator<GetSYYDiagnosisTypeDefsQuery>
{
 public GetSYYDiagnosisTypeDefsQueryValidator(){RuleFor(x=>x.PageNumber).GreaterThan(0);
 RuleFor(x=>x.PageSize).InclusiveBetween(1,100);RuleFor(x=>x.Search).MaximumLength(250);}
}
