using MediatR;
using KLMN.Application.SYYDiagnosisBranches.Responses;
namespace KLMN.Application.SYYDiagnosisBranches.Commands.CreateSYYDiagnosisBranchDef;
/// <summary>SYY Gereken Branşlar Create CQRS komutu.</summary>
public sealed record CreateSYYDiagnosisBranchDefCommand(Guid SYYDiagnosisDefId,Guid BranchDefId,int StartingAge,int EndingAge,int Gender):IRequest<SYYDiagnosisBranchDefMutationResponse>;
