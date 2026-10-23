using System.Linq.Expressions;
using KLMN.Domain.Entities.Admissions;
namespace KLMN.Application.ICD10Defs.Responses;
/// <summary>ICD10 Tanımları: adların FK ile EF projeksiyonundan okunması ve xmin sürümü.</summary>
public sealed record ICD10DefResponse(Guid Id,string Code,string Name,bool IsActive,uint Version,DateTime CreatedDate)
{
 public static Expression<Func<ICD10Def,ICD10DefResponse>> Projection=>x=>
 new(x.Id,x.Code,x.Name,x.IsActive,x.Version,x.CreatedDate);
}
public sealed record ICD10DefMutationResponse(Guid Id,uint Version);
