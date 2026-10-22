using System.Linq.Expressions;
using KLMN.Domain.Entities.Admissions;
namespace KLMN.Application.FlyingMissions.Responses;
/// <summary>Uçucu Görev Tanımları liste/detay DTO'su, xmin sürümü ve bağlı adlar.</summary>
public sealed record FlyingMissionDefResponse(Guid Id,string Code,string Name,DateTime? ValidFrom,DateTime? ValidTo,bool IsActive,uint Version,DateTime CreatedDate)
{
 public static Expression<Func<FlyingMissionDef,FlyingMissionDefResponse>> Projection=>x=>new(
 x.Id,x.Code,x.Name,x.ValidFrom,x.ValidTo,x.IsActive,x.Version,x.CreatedDate);
}
/// <summary>Kaydedilen nesnenin Id ve xmin sürümü.</summary>
public sealed record FlyingMissionDefMutationResponse(Guid Id,uint Version);
