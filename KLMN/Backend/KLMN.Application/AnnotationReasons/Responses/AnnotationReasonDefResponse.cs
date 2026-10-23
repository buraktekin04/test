using KLMN.Domain.Entities.Admissions;
namespace KLMN.Application.AnnotationReasons.Responses;
/// <summary>Sevk Sebebi Tanımları liste/detay DTO'su, xmin sürümü ve bağlı adlar.</summary>
public sealed record AnnotationReasonDefResponse(Guid Id,string Code,string Name,DateTime? ValidFrom,DateTime? ValidTo,bool IsActive,uint Version,DateTime CreatedDate)
{

}
