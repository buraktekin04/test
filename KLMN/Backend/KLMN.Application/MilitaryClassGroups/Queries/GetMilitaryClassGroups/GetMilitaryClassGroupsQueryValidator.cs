using FluentValidation;

namespace KLMN.Application.MilitaryClassGroups.Queries.GetMilitaryClassGroups;

/// <summary>Grup araması ve sayfalama sınırlarıdır.</summary>
public sealed class GetMilitaryClassGroupsQueryValidator : AbstractValidator<GetMilitaryClassGroupsQuery>
{
    /// <summary>Geçersiz filtre ve aşırı sayfalama reddedilir.</summary>
    public GetMilitaryClassGroupsQueryValidator()
    {
        RuleFor(x => x.Search).MaximumLength(250);
        RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        RuleFor(x => x.ForcesCommandId).Must(x => !x.HasValue || x.Value != Guid.Empty);
    }
}
