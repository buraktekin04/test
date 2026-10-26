using KLMN.Application.HcDecisions.Common;

using MediatR;
namespace KLMN.Application.HcDecisionConnections.Queries.GetHcConnectedDecisionOptions;
/// <summary>Bağlı Kararlar: aktif seçimler sorgusu.</summary>
public sealed record GetHcConnectedDecisionOptionsQuery(Guid? SelectedDecisionId=null,Guid? SelectedReferenceId=null):IRequest<HcDecisionRelationOptions>;
