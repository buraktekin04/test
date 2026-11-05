using KLMN.Application.Provisions.Responses;
using MediatR;

namespace KLMN.Application.Provisions.Queries.GetProvisionStates;

public sealed record GetProvisionStatesQuery(Guid Id):IRequest<IReadOnlyList<ProvisionStateResponse>>;
