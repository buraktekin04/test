using FluentValidation;
using KLMN.Application.Common.Models;

namespace KLMN.Application.Roles.Queries.GetRoles;

/// <summary>Listeleme boyutu ve arama terimlerini doğrular.</summary>
public sealed class GetRolesQueryValidator : AbstractValidator<GetRolesQuery>
{
    /// <summary>Sunucu filtrelerine uygulanan güvenli sınırları tanımlar.</summary>
    public GetRolesQueryValidator()
    {
        RuleFor(x => x.SortField).Must(GetRolesSorting.Definition.Supports)
            .WithMessage("Geçersiz sıralama kolonu.");
        RuleFor(x => x.SortOrder).Must(order => order == 1 || order == -1)
            .WithMessage("Sıralama yönü 1 veya -1 olmalıdır.");
        RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        RuleFor(x => x.Search).MaximumLength(200);
    }
}
