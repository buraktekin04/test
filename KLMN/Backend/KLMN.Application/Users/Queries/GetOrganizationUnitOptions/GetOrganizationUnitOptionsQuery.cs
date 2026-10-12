using KLMN.Application.Users.Responses;
using MediatR;

namespace KLMN.Application.Users.Queries.GetOrganizationUnitOptions;

/// <summary>
/// Kullanıcı görev yeri seçiminde kullanılmak üzere aktif organizasyon
/// birimlerini döndürür; rol veya yetki bilgilerini içermez.
/// </summary>
public sealed record GetOrganizationUnitOptionsQuery
    : IRequest<IReadOnlyList<OrganizationUnitOptionResponse>>;
