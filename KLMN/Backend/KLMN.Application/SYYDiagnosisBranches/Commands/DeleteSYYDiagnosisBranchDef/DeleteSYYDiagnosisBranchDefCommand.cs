using MediatR;

namespace KLMN.Application.SYYDiagnosisBranches.Commands.DeleteSYYDiagnosisBranchDef;
/// <summary>SYY Gereken Branşlar Delete CQRS komutu.</summary>
public sealed record DeleteSYYDiagnosisBranchDefCommand(Guid Id,uint Version):IRequest;
