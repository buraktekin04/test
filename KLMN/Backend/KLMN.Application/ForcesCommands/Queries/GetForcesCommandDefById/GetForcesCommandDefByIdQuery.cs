using KLMN.Application.ForcesCommands.Responses;
using MediatR;

namespace KLMN.Application.ForcesCommands.Queries.GetForcesCommandDefById;

/// <summary>Güncel kuvvet komutanlığı kaydını ve optimistic concurrency versiyonunu ister.</summary>
public sealed record GetForcesCommandDefByIdQuery(Guid Id) : IRequest<ForcesCommandDefResponse>;
