using System.Linq.Expressions;
using KLMN.Domain.Entities.Admissions;
namespace KLMN.Application.PlaneTypes.Responses;
/// <summary>Uçuşa Uçak Tipi Tanımları liste/detay DTO'su, xmin sürümü ve bağlı adlar.</summary>
public sealed record PlaneTypeDefResponse(Guid Id,string Code,string Name,DateTime? ValidFrom,DateTime? ValidTo,bool IsActive,uint Version,DateTime CreatedDate)
{
 public static Expression<Func<PlaneTypeDef,PlaneTypeDefResponse>> Projection=>x=>new(
 x.Id,x.Code,x.Name,x.ValidFrom,x.ValidTo,x.IsActive,x.Version,x.CreatedDate);
}
/// <summary>Kaydedilen nesnenin Id ve xmin sürümü.</summary>
public sealed record PlaneTypeDefMutationResponse(Guid Id,uint Version);
