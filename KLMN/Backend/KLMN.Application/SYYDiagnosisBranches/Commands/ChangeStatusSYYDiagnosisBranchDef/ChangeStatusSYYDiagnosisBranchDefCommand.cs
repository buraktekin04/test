using MediatR;
using KLMN.Application.SYYDiagnosisBranches.Responses;
namespace KLMN.Application.SYYDiagnosisBranches.Commands.ChangeStatusSYYDiagnosisBranchDef;
/// <summary>SYY Gereken Branşlar ChangeStatus CQRS komutu.</summary>
public sealed record ChangeStatusSYYDiagnosisBranchDefCommand(Guid Id,uint Version,bool IsActive):IRequest<SYYDiagnosisBranchDefMutationResponse>;
