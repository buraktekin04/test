using KLMN.Application.Common.Models;
using KLMN.Application.ICD10Diagnostics.Responses;
using MediatR;
namespace KLMN.Application.ICD10Diagnostics.Queries.GetICD10Diagnostics;
/// <summary>ICD10 SYY Tanı İstatistik Tanımları: filtre ve sayfalama modeli.</summary>
public sealed record GetICD10DiagnosticsQuery(string? Search=null,bool IncludeInactive=false,
 bool? IsActive=null,int PageNumber=1,int PageSize=20,Guid? ICD10DefId=null,Guid? SYYDiagnosisDefId=null,Guid? HcAdmissionTypeDefId=null)
 :IRequest<PagedResult<ICD10DiagnosticResponse>>;
