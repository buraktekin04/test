using MediatR;

namespace KLMN.Application.Branches.Commands.DeleteBranchDef;

/// <summary>Branş Delete CQRS komutu.</summary>
public sealed record DeleteBranchDefCommand(Guid Id, uint Version) : IRequest;
