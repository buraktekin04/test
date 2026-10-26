using KLMN.Application.HcUnusedReasons.Responses;

using MediatR;
namespace KLMN.Application.HcUnusedReasons.Queries.GetHcUnusedReasonForExamById;
/// <summary>Kullanılmayacak Muayene Sebepleri: tek kayıt sorgusu.</summary>
public sealed record GetHcUnusedReasonForExamByIdQuery(Guid Id):IRequest<HcUnusedReasonForExamResponse>;
