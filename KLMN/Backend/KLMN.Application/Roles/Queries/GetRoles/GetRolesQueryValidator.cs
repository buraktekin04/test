using FluentValidation;
using KLMN.Application.Common.Models;

namespace KLMN.Application.Roles.Queries.GetRoles;

/// <summary>Listeleme boyutu ve arama terimlerini doğrular.</summary>
public sealed class GetRolesQueryValidator : AbstractValidator<GetRolesQuery>
{
    /// <summary>Sunucu filtrelerine uygulanan güvenli sınırları tanımlar.</summary>
    public GetRolesQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        RuleFor(x => x.Search).MaximumLength(200);
    }
}
