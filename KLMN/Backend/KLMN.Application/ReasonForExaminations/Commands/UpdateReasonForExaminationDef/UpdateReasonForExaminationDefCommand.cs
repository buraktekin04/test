using MediatR;
using KLMN.Application.ReasonForExaminations.Responses;
namespace KLMN.Application.ReasonForExaminations.Commands.UpdateReasonForExaminationDef;
/// <summary>Update Muayene Sebebi CQRS komutu.</summary>
public sealed record UpdateReasonForExaminationDefCommand(Guid Id,uint Version,string Code,string Description,bool DispatchRequired,bool ClassificationExamination,bool Objectionable,DateTime? ValidFrom,DateTime? ValidTo) : IRequest<ReasonForExaminationDefMutationResponse>;
