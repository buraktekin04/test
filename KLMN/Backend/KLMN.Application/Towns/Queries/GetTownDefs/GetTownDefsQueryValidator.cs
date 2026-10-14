using FluentValidation;
namespace KLMN.Application.Towns.Queries.GetTownDefs;
/// <summary>İlçe tanımına ait sorgu, yanıt veya iş kuralı bileşeni.</summary>
public sealed class GetTownDefsQueryValidator : AbstractValidator<GetTownDefsQuery>
{
    public GetTownDefsQueryValidator()
    {
        RuleFor(x => x.Search).MaximumLength(250);
        RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}
