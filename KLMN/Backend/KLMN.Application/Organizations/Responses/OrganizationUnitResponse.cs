using System.Linq.Expressions;
using KLMN.Domain.Entities.Organization;

namespace KLMN.Application.Organizations.Responses;

/// <summary>Organizasyon tanımları liste/detay ekranındaki güvenli birim yanıtıdır.</summary>
public sealed class OrganizationUnitResponse
{
    public Guid Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public Guid? ParentOrganizationUnitId { get; init; }
    public string? ParentOrganizationUnitName { get; init; }
    public string? ParentOrganizationUnitCode { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedDate { get; init; }
    public uint Version { get; init; }
    public int UserCount { get; init; }
    public int ChildCount { get; init; }

    /// <summary>EF Core'un SQL'e çevirebildiği tekil DTO projeksiyonudur.</summary>
    public static Expression<Func<OrganizationUnit, OrganizationUnitResponse>> Projection => unit =>
        new OrganizationUnitResponse
        {
            Id = unit.Id,
            Code = unit.Code,
            Name = unit.Name,
            ParentOrganizationUnitId = unit.ParentOrganizationUnitId,
            ParentOrganizationUnitName = unit.ParentOrganizationUnit != null
                ? unit.ParentOrganizationUnit.Name : null,
            ParentOrganizationUnitCode = unit.ParentOrganizationUnit != null
                ? unit.ParentOrganizationUnit.Code : null,
            IsActive = unit.IsActive,
            CreatedDate = unit.CreatedDate,
            Version = unit.Version,
            UserCount = unit.Users.Count(x => !x.IsDeleted),
            ChildCount = unit.Children.Count(x => !x.IsDeleted)
        };
}
