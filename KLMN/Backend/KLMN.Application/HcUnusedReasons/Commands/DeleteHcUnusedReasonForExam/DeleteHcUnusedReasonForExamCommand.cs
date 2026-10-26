using MediatR;

namespace KLMN.Application.HcUnusedReasons.Commands.DeleteHcUnusedReasonForExam;
/// <summary>Kullanılmayacak Muayene Sebepleri için bağımsız Delete isteği.</summary>
public sealed record DeleteHcUnusedReasonForExamCommand(Guid Id,uint Version):IRequest;
