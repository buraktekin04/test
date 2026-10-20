using KLMN.Application.ReasonForExamUnusedHcAdmissionTypes.Responses;
using MediatR;
namespace KLMN.Application.ReasonForExamUnusedHcAdmissionTypes.Queries.GetReasonForExamUnusedHcAdmissionTypeById;
/// <summary>İlişki detayını GUID ile ister.</summary>
public sealed record GetReasonForExamUnusedHcAdmissionTypeByIdQuery(Guid Id):IRequest<ReasonForExamUnusedHcAdmissionTypeResponse>;
