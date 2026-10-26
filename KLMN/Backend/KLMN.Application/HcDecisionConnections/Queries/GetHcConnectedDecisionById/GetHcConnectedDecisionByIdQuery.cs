using KLMN.Application.HcDecisionConnections.Responses;

using MediatR;
namespace KLMN.Application.HcDecisionConnections.Queries.GetHcConnectedDecisionById;
/// <summary>Bağlı Kararlar: tek kayıt sorgusu.</summary>
public sealed record GetHcConnectedDecisionByIdQuery(Guid Id):IRequest<HcConnectedDecisionResponse>;
