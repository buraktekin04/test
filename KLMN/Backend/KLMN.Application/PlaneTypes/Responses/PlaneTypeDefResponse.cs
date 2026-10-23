using KLMN.Domain.Entities.Admissions;
namespace KLMN.Application.PlaneTypes.Responses;
/// <summary>Uçuşa Uçak Tipi Tanımları liste/detay DTO'su, xmin sürümü ve bağlı adlar.</summary>
public sealed record PlaneTypeDefResponse(Guid Id,string Code,string Name,DateTime? ValidFrom,DateTime? ValidTo,bool IsActive,uint Version,DateTime CreatedDate)
{

}
