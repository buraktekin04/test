using MediatR;
using KLMN.Application.ReasonForExamBranchDefs.Responses;
namespace KLMN.Application.ReasonForExamBranchDefs.Commands.UpdateReasonForExamBranchDef;
/// <summary>Muayene Gereken Branşlar: Update CQRS komutu.</summary>
public sealed record UpdateReasonForExamBranchDefCommand(Guid Id,uint Version,Guid ReasonForExaminationDefId,Guid ReferenceId,int StartingAge,int EndingAge,int Gender):IRequest<ReasonForExamBranchDefMutationResponse>;
