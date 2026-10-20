using KLMN.Application.ReasonForExaminations.Common;
using MediatR;
namespace KLMN.Application.ReasonForExamBranchDefs.Queries.GetReasonForExamBranchDefOptions;
/// <summary>Modülün kendi izniyle ana muayene sebeplerini ve referans kayıtlarını listeler.</summary>
public sealed record GetReasonForExamBranchDefOptionsQuery(Guid? SelectedReasonId=null,Guid? SelectedReferenceId=null)
 :IRequest<ReasonRelationOptions>;
