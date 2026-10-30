using FluentValidation;
namespace KLMN.Application.Dispatches.Queries.GetDispatches;
/// <summary>Sorguda ağır ve sınırsız veri erişimini önler.</summary>
public sealed class GetDispatchesQueryValidator:AbstractValidator<GetDispatchesQuery>
{
    public GetDispatchesQueryValidator()
    {
        RuleFor(x=>x.PageNumber).GreaterThan(0);
        RuleFor(x=>x.PageSize).InclusiveBetween(1,100);
        RuleFor(x=>x.Search).MaximumLength(100);
    }
}
