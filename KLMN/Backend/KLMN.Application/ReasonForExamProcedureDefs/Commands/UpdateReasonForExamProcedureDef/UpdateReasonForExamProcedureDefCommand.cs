using MediatR;
using KLMN.Application.ReasonForExamProcedureDefs.Responses;
namespace KLMN.Application.ReasonForExamProcedureDefs.Commands.UpdateReasonForExamProcedureDef;
/// <summary>Gereken Tetkikler: Update CQRS komutu.</summary>
public sealed record UpdateReasonForExamProcedureDefCommand(Guid Id,uint Version,Guid ReasonForExaminationDefId,Guid ReferenceId,int StartingAge,int EndingAge,int Gender):IRequest<ReasonForExamProcedureDefMutationResponse>;
