using MediatR;
using KLMN.Application.HcUnusedReasons.Responses;
namespace KLMN.Application.HcUnusedReasons.Commands.ChangeStatusHcUnusedReasonForExam;
/// <summary>Kullanılmayacak Muayene Sebepleri için bağımsız ChangeStatus isteği.</summary>
public sealed record ChangeStatusHcUnusedReasonForExamCommand(Guid Id,uint Version,bool IsActive):IRequest<HcUnusedReasonForExamMutationResponse>;
