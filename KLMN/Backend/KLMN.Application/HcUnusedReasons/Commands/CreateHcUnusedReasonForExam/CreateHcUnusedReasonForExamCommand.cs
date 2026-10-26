using MediatR;
using KLMN.Application.HcUnusedReasons.Responses;
namespace KLMN.Application.HcUnusedReasons.Commands.CreateHcUnusedReasonForExam;
/// <summary>Kullanılmayacak Muayene Sebepleri için bağımsız Create isteği.</summary>
public sealed record CreateHcUnusedReasonForExamCommand(Guid HcDecisionDefId,Guid ReferenceId):IRequest<HcUnusedReasonForExamMutationResponse>;
