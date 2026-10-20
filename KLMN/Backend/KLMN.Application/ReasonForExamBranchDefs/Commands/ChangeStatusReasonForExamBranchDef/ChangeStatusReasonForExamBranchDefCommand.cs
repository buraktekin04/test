using MediatR;
using KLMN.Application.ReasonForExamBranchDefs.Responses;
namespace KLMN.Application.ReasonForExamBranchDefs.Commands.ChangeStatusReasonForExamBranchDef;
/// <summary>Muayene Gereken Branşlar: ChangeStatus CQRS komutu.</summary>
public sealed record ChangeStatusReasonForExamBranchDefCommand(Guid Id,uint Version,bool IsActive):IRequest<ReasonForExamBranchDefMutationResponse>;
