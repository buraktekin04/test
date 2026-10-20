using KLMN.Application.ReasonForExaminations.Common;
using MediatR;
namespace KLMN.Application.ReasonForExamProcedureDefs.Queries.GetReasonForExamProcedureDefOptions;
/// <summary>Modülün kendi izniyle ana muayene sebeplerini ve referans kayıtlarını listeler.</summary>
public sealed record GetReasonForExamProcedureDefOptionsQuery(Guid? SelectedReasonId=null,Guid? SelectedReferenceId=null)
 :IRequest<ReasonRelationOptions>;
