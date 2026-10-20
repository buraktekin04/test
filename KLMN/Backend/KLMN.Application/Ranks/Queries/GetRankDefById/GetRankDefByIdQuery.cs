using KLMN.Application.Ranks.Responses;
using MediatR;

namespace KLMN.Application.Ranks.Queries.GetRankDefById;

/// <summary>xmin içeren rütbe detay sorgusu.</summary>
public sealed record GetRankDefByIdQuery(Guid Id) : IRequest<RankDefResponse>;
