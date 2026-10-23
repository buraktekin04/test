using KLMN.Domain.Entities.Admissions;
namespace KLMN.Application.SYYDiagnosisSlices.Responses;
/// <summary>SYY Dilim Tanımları liste/detay DTO'su, xmin sürümü ve bağlı adlar.</summary>
public sealed record SYYDiagnosisSliceDefResponse(Guid Id,int Code,string Name,bool IsActive,uint Version,DateTime CreatedDate)
{

}
