using KLMN.Application.RankGroups.Responses;
using MediatR;

namespace KLMN.Application.RankGroups.Queries.GetRankGroupDefById;

/// <summary>xmin içeren rütbe grubu detay sorgusu.</summary>
public sealed record GetRankGroupDefByIdQuery(Guid Id) : IRequest<RankGroupDefResponse>;
