using MediatR;

namespace KLMN.Application.ReasonForExamBranchDefs.Commands.DeleteReasonForExamBranchDef;
/// <summary>Muayene Gereken Branşlar: Delete CQRS komutu.</summary>
public sealed record DeleteReasonForExamBranchDefCommand(Guid Id,uint Version):IRequest;
