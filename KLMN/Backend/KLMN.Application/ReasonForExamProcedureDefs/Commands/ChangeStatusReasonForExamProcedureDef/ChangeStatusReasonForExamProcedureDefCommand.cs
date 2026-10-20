using MediatR;
using KLMN.Application.ReasonForExamProcedureDefs.Responses;
namespace KLMN.Application.ReasonForExamProcedureDefs.Commands.ChangeStatusReasonForExamProcedureDef;
/// <summary>Gereken Tetkikler: ChangeStatus CQRS komutu.</summary>
public sealed record ChangeStatusReasonForExamProcedureDefCommand(Guid Id,uint Version,bool IsActive):IRequest<ReasonForExamProcedureDefMutationResponse>;
