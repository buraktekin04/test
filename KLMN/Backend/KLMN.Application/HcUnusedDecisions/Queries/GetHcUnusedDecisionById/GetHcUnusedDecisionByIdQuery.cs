using KLMN.Application.HcUnusedDecisions.Responses;

using MediatR;
namespace KLMN.Application.HcUnusedDecisions.Queries.GetHcUnusedDecisionById;
/// <summary>Birlikte Verilmeyecek Kararlar: tek kayıt sorgusu.</summary>
public sealed record GetHcUnusedDecisionByIdQuery(Guid Id):IRequest<HcUnusedDecisionResponse>;
