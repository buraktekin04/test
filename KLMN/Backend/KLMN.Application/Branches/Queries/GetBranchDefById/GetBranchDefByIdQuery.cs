using KLMN.Application.Branches.Responses;
using MediatR;

namespace KLMN.Application.Branches.Queries.GetBranchDefById;

/// <summary>xmin içeren branş detay sorgusu.</summary>
public sealed record GetBranchDefByIdQuery(Guid Id) : IRequest<BranchDefResponse>;
