using MediatR;

namespace KLMN.Application.ReasonForExaminations.Commands.DeleteReasonForExaminationDef;
/// <summary>Delete Muayene Sebebi CQRS komutu.</summary>
public sealed record DeleteReasonForExaminationDefCommand(Guid Id,uint Version) : IRequest;
