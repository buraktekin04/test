using MediatR;
using KLMN.Application.ReasonForExaminations.Responses;
namespace KLMN.Application.ReasonForExaminations.Commands.CreateReasonForExaminationDef;
/// <summary>Create Muayene Sebebi CQRS komutu.</summary>
public sealed record CreateReasonForExaminationDefCommand(string Code,string Description,bool DispatchRequired,bool ClassificationExamination,bool Objectionable,DateTime? ValidFrom,DateTime? ValidTo) : IRequest<ReasonForExaminationDefMutationResponse>;
