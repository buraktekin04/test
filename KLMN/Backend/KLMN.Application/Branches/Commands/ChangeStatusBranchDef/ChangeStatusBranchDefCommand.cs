using KLMN.Application.Branches.Responses;
using MediatR;

namespace KLMN.Application.Branches.Commands.ChangeStatusBranchDef;

/// <summary>Branş ChangeStatus CQRS komutu.</summary>
public sealed record ChangeStatusBranchDefCommand(Guid Id, uint Version, bool IsActive) : IRequest<BranchDefMutationResponse>;
