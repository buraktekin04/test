using KLMN.Application.ReasonForExamReportTypes.Responses;
using MediatR;
namespace KLMN.Application.ReasonForExamReportTypes.Queries.GetReasonForExamReportTypeById;
/// <summary>İlişki detayını GUID ile ister.</summary>
public sealed record GetReasonForExamReportTypeByIdQuery(Guid Id):IRequest<ReasonForExamReportTypeResponse>;
