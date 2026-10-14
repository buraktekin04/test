using FluentValidation;

namespace KLMN.Application.PurchaseNumbers.Queries.GetPurchaseNumberDefs;

public sealed class GetPurchaseNumberDefsQueryValidator : AbstractValidator<GetPurchaseNumberDefsQuery>
{
    public GetPurchaseNumberDefsQueryValidator()
    {
        RuleFor(x => x.Search).MaximumLength(250);
        RuleFor(x => x.SupplyDescription).MaximumLength(500);
        RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}
