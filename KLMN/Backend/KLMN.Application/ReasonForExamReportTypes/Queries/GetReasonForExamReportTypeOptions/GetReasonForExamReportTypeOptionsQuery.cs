using KLMN.Application.ReasonForExaminations.Common;
using MediatR;
namespace KLMN.Application.ReasonForExamReportTypes.Queries.GetReasonForExamReportTypeOptions;
/// <summary>Modülün kendi izniyle ana muayene sebeplerini ve referans kayıtlarını listeler.</summary>
public sealed record GetReasonForExamReportTypeOptionsQuery(Guid? SelectedReasonId=null,Guid? SelectedReferenceId=null)
 :IRequest<ReasonRelationOptions>;
