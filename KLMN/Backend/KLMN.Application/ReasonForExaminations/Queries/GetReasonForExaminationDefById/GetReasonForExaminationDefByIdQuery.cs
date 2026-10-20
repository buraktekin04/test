using KLMN.Application.ReasonForExaminations.Responses;
using MediatR;
namespace KLMN.Application.ReasonForExaminations.Queries.GetReasonForExaminationDefById;
/// <summary>Muayene sebebi GUID detay sorgusu.</summary>
public sealed record GetReasonForExaminationDefByIdQuery(Guid Id):IRequest<ReasonForExaminationDefResponse>;
