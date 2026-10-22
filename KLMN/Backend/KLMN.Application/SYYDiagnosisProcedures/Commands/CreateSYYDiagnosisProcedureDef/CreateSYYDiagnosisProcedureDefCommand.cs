using MediatR;
using KLMN.Application.SYYDiagnosisProcedures.Responses;
namespace KLMN.Application.SYYDiagnosisProcedures.Commands.CreateSYYDiagnosisProcedureDef;
/// <summary>SYY Gereken Tetkikler Create CQRS komutu.</summary>
public sealed record CreateSYYDiagnosisProcedureDefCommand(Guid SYYDiagnosisDefId,Guid ProcedureDefId,int StartingAge,int EndingAge,int Gender):IRequest<SYYDiagnosisProcedureDefMutationResponse>;
