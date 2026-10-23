using KLMN.Domain.Entities.Admissions;
namespace KLMN.Application.ICD10Defs.Responses;
/// <summary>ICD10 Tanımları: adların FK ile EF projeksiyonundan okunması ve xmin sürümü.</summary>
public sealed record ICD10DefResponse(Guid Id,string Code,string Name,bool IsActive,uint Version,DateTime CreatedDate)
{

}
