using KLMN.Application.Common.Models;
using KLMN.Application.SYYDiagnosisDecisions.Responses;
using MediatR;
namespace KLMN.Application.SYYDiagnosisDecisions.Queries.GetSYYDiagnosisDecisions;
/// <summary>Tanımlama ve sorgulama ekranları için sunucu taraflı filtrelenmiş/sayfalı sorgu.</summary>
public sealed record GetSYYDiagnosisDecisionsQuery(
 string? Search=null,bool IncludeInactive=false,bool? IsActive=null,
 Guid? SYYDiagnosisDefId=null,Guid? HcAdmissionTypeDefId=null,
 Guid? MilitaryClassDefId=null,Guid? HcDecisionDefId=null,
 int PageNumber=1,int PageSize=20)
 :IRequest<PagedResult<SYYDiagnosisDecisionResponse>>;
