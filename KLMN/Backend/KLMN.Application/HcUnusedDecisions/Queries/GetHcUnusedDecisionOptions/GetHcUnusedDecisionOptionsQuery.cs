using KLMN.Application.HcDecisions.Common;

using MediatR;
namespace KLMN.Application.HcUnusedDecisions.Queries.GetHcUnusedDecisionOptions;
/// <summary>Birlikte Verilmeyecek Kararlar: aktif seçimler sorgusu.</summary>
public sealed record GetHcUnusedDecisionOptionsQuery(Guid? SelectedDecisionId=null,Guid? SelectedReferenceId=null):IRequest<HcDecisionRelationOptions>;
