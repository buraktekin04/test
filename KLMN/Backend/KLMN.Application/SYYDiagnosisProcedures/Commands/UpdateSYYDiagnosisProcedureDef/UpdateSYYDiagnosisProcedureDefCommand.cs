using MediatR;
using KLMN.Application.SYYDiagnosisProcedures.Responses;
namespace KLMN.Application.SYYDiagnosisProcedures.Commands.UpdateSYYDiagnosisProcedureDef;
/// <summary>SYY Gereken Tetkikler Update CQRS komutu.</summary>
public sealed record UpdateSYYDiagnosisProcedureDefCommand(Guid Id,uint Version,Guid SYYDiagnosisDefId,Guid ProcedureDefId,int StartingAge,int EndingAge,int Gender):IRequest<SYYDiagnosisProcedureDefMutationResponse>;
