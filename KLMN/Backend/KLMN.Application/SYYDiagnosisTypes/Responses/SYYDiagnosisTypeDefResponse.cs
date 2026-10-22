using System.Linq.Expressions;
using KLMN.Domain.Entities.Admissions;
namespace KLMN.Application.SYYDiagnosisTypes.Responses;
/// <summary>SYY Tanı Türü Tanımları liste/detay DTO'su, xmin sürümü ve bağlı adlar.</summary>
public sealed record SYYDiagnosisTypeDefResponse(Guid Id,int Code,string Name,bool IsActive,uint Version,DateTime CreatedDate)
{
 public static Expression<Func<SYYDiagnosisTypeDef,SYYDiagnosisTypeDefResponse>> Projection=>x=>new(
 x.Id,x.Code,x.Name,x.IsActive,x.Version,x.CreatedDate);
}
/// <summary>Kaydedilen nesnenin Id ve xmin sürümü.</summary>
public sealed record SYYDiagnosisTypeDefMutationResponse(Guid Id,uint Version);
