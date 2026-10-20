using MediatR;
using KLMN.Application.ReasonForExamBranchDefs.Responses;
namespace KLMN.Application.ReasonForExamBranchDefs.Commands.CreateReasonForExamBranchDef;
/// <summary>Muayene Gereken Branşlar: Create CQRS komutu.</summary>
public sealed record CreateReasonForExamBranchDefCommand(Guid ReasonForExaminationDefId,Guid ReferenceId,int StartingAge,int EndingAge,int Gender):IRequest<ReasonForExamBranchDefMutationResponse>;
