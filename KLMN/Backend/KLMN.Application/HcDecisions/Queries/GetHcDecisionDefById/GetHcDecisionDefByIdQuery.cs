using KLMN.Application.HcDecisions.Responses;
using MediatR;
namespace KLMN.Application.HcDecisions.Queries.GetHcDecisionDefById;
/// <summary>Kurul kararı tek kayıt sorgusu.</summary>
public sealed record GetHcDecisionDefByIdQuery(Guid Id)
  :IRequest<HcDecisionDefResponse>;
