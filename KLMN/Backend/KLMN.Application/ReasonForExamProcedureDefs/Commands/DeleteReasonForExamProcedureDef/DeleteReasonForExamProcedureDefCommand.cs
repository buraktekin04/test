using MediatR;

namespace KLMN.Application.ReasonForExamProcedureDefs.Commands.DeleteReasonForExamProcedureDef;
/// <summary>Gereken Tetkikler: Delete CQRS komutu.</summary>
public sealed record DeleteReasonForExamProcedureDefCommand(Guid Id,uint Version):IRequest;
