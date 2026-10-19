using KLMN.Application.Branches.Responses;
using MediatR;

namespace KLMN.Application.Branches.Commands.UpdateBranchDef;

/// <summary>Branş Update CQRS komutu.</summary>
public sealed record UpdateBranchDefCommand(Guid Id, uint Version, string Code, string Name, DateTime? ValidFrom, DateTime? ValidTo) : IRequest<BranchDefMutationResponse>;
