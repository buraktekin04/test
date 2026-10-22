using MediatR;
using KLMN.Application.SYYDiagnosisBranches.Responses;
namespace KLMN.Application.SYYDiagnosisBranches.Commands.UpdateSYYDiagnosisBranchDef;
/// <summary>SYY Gereken Branşlar Update CQRS komutu.</summary>
public sealed record UpdateSYYDiagnosisBranchDefCommand(Guid Id,uint Version,Guid SYYDiagnosisDefId,Guid BranchDefId,int StartingAge,int EndingAge,int Gender):IRequest<SYYDiagnosisBranchDefMutationResponse>;
