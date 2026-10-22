using FluentValidation;
namespace KLMN.Application.SYYClassifications.Queries.GetSYYClassificationDefs;
public sealed class GetSYYClassificationDefsQueryValidator:AbstractValidator<GetSYYClassificationDefsQuery>
{
 public GetSYYClassificationDefsQueryValidator()
 {RuleFor(x=>x.PageNumber).GreaterThan(0);RuleFor(x=>x.PageSize).InclusiveBetween(1,100);
 RuleFor(x=>x.Search).MaximumLength(250);}
}
