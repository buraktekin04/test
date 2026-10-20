using KLMN.Application.ReasonForExamBranchDefs.Responses;
using MediatR;
namespace KLMN.Application.ReasonForExamBranchDefs.Queries.GetReasonForExamBranchDefById;
/// <summary>İlişki detayını GUID ile ister.</summary>
public sealed record GetReasonForExamBranchDefByIdQuery(Guid Id):IRequest<ReasonForExamBranchDefResponse>;
