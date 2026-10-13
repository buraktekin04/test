using KLMN.Application.Organizations.Responses;
using MediatR;

namespace KLMN.Application.Organizations.Queries.GetOrganizationUnitById;

/// <summary>Güncel organizasyon kaydını ve optimistic concurrency versiyonunu ister.</summary>
public sealed record GetOrganizationUnitByIdQuery(Guid Id) : IRequest<OrganizationUnitResponse>;
