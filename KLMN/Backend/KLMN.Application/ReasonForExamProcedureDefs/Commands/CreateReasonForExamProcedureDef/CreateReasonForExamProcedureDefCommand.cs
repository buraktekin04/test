using MediatR;
using KLMN.Application.ReasonForExamProcedureDefs.Responses;
namespace KLMN.Application.ReasonForExamProcedureDefs.Commands.CreateReasonForExamProcedureDef;
/// <summary>Gereken Tetkikler: Create CQRS komutu.</summary>
public sealed record CreateReasonForExamProcedureDefCommand(Guid ReasonForExaminationDefId,Guid ReferenceId,int StartingAge,int EndingAge,int Gender):IRequest<ReasonForExamProcedureDefMutationResponse>;
