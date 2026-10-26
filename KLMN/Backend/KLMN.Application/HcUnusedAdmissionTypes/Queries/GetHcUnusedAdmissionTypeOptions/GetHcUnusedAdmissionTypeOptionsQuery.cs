using KLMN.Application.HcDecisions.Common;

using MediatR;
namespace KLMN.Application.HcUnusedAdmissionTypes.Queries.GetHcUnusedAdmissionTypeOptions;
/// <summary>Kullanılmayacak Başvuru Grupları: aktif seçimler sorgusu.</summary>
public sealed record GetHcUnusedAdmissionTypeOptionsQuery(Guid? SelectedDecisionId=null,Guid? SelectedReferenceId=null):IRequest<HcDecisionRelationOptions>;
