using KLMN.Application.ReasonForExamProcedureDefs.Responses;
using MediatR;
namespace KLMN.Application.ReasonForExamProcedureDefs.Queries.GetReasonForExamProcedureDefById;
/// <summary>İlişki detayını GUID ile ister.</summary>
public sealed record GetReasonForExamProcedureDefByIdQuery(Guid Id):IRequest<ReasonForExamProcedureDefResponse>;
