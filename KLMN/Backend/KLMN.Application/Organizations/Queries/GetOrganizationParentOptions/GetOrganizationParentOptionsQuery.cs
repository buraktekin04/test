using KLMN.Application.Organizations.Responses;
using MediatR;

namespace KLMN.Application.Organizations.Queries.GetOrganizationParentOptions;

/// <summary>Üst birim seçiminde kullanılabilen aktif organizasyon kayıtlarını ister.</summary>
public sealed record GetOrganizationParentOptionsQuery()
    : IRequest<IReadOnlyList<OrganizationParentOptionResponse>>;
