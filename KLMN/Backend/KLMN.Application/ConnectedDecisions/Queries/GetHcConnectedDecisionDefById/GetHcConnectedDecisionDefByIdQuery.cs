using KLMN.Application.ConnectedDecisions.Responses;
using MediatR;

namespace KLMN.Application.ConnectedDecisions.Queries.GetHcConnectedDecisionDefById;

/// <summary>xmin içeren bağlı kurul kararı detay sorgusu.</summary>
public sealed record GetHcConnectedDecisionDefByIdQuery(Guid Id) : IRequest<HcConnectedDecisionDefResponse>;
