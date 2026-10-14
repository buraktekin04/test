using FluentValidation;

namespace KLMN.Application.ForcesCommands.Queries.GetForcesCommandDefs;

/// <summary>Arama ve sayfa sınırlarını doğrular.</summary>
public sealed class GetForcesCommandDefsQueryValidator : AbstractValidator<GetForcesCommandDefsQuery>
{
    public GetForcesCommandDefsQueryValidator()
    {
        RuleFor(x => x.Search).MaximumLength(250);
        RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}
