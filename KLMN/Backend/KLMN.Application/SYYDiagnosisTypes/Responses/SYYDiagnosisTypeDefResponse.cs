using KLMN.Domain.Entities.Admissions;
namespace KLMN.Application.SYYDiagnosisTypes.Responses;
/// <summary>SYY Tanı Türü Tanımları liste/detay DTO'su, xmin sürümü ve bağlı adlar.</summary>
public sealed record SYYDiagnosisTypeDefResponse(Guid Id,int Code,string Name,bool IsActive,uint Version,DateTime CreatedDate)
{

}
