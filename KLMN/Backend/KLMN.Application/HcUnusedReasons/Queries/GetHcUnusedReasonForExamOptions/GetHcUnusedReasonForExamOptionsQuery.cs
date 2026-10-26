using KLMN.Application.HcDecisions.Common;

using MediatR;
namespace KLMN.Application.HcUnusedReasons.Queries.GetHcUnusedReasonForExamOptions;
/// <summary>Kullanılmayacak Muayene Sebepleri: aktif seçimler sorgusu.</summary>
public sealed record GetHcUnusedReasonForExamOptionsQuery(Guid? SelectedDecisionId=null,Guid? SelectedReferenceId=null):IRequest<HcDecisionRelationOptions>;
