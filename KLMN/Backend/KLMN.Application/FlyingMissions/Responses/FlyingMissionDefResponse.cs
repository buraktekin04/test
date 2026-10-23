using KLMN.Domain.Entities.Admissions;
namespace KLMN.Application.FlyingMissions.Responses;
/// <summary>Uçucu Görev Tanımları liste/detay DTO'su, xmin sürümü ve bağlı adlar.</summary>
public sealed record FlyingMissionDefResponse(Guid Id,string Code,string Name,DateTime? ValidFrom,DateTime? ValidTo,bool IsActive,uint Version,DateTime CreatedDate)
{

}
