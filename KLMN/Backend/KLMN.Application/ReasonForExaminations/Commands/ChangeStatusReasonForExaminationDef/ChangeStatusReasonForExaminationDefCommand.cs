using MediatR;
using KLMN.Application.ReasonForExaminations.Responses;
namespace KLMN.Application.ReasonForExaminations.Commands.ChangeStatusReasonForExaminationDef;
/// <summary>ChangeStatus Muayene Sebebi CQRS komutu.</summary>
public sealed record ChangeStatusReasonForExaminationDefCommand(Guid Id,uint Version,bool IsActive) : IRequest<ReasonForExaminationDefMutationResponse>;
