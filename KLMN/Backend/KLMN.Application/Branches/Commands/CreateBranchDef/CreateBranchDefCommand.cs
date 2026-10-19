using KLMN.Application.Branches.Responses;
using MediatR;

namespace KLMN.Application.Branches.Commands.CreateBranchDef;

/// <summary>Branş Create CQRS komutu.</summary>
public sealed record CreateBranchDefCommand(string Code, string Name, DateTime? ValidFrom, DateTime? ValidTo) : IRequest<BranchDefMutationResponse>;
