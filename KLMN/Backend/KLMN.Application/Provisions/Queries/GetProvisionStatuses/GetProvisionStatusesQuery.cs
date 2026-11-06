using KLMN.Application.Provisions.Responses;
using MediatR;

namespace KLMN.Application.Provisions.Queries.GetProvisionStatuses;

public sealed record GetProvisionStatusesQuery() : IRequest<IReadOnlyList<ProvisionStatusOption>>;
