using MediatR;
using KLMN.Application.HcUnusedReasons.Responses;
namespace KLMN.Application.HcUnusedReasons.Commands.UpdateHcUnusedReasonForExam;
/// <summary>Kullanılmayacak Muayene Sebepleri için bağımsız Update isteği.</summary>
public sealed record UpdateHcUnusedReasonForExamCommand(Guid Id,uint Version,Guid HcDecisionDefId,Guid ReferenceId):IRequest<HcUnusedReasonForExamMutationResponse>;
