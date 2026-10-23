using KLMN.Domain.Entities.Admissions;
namespace KLMN.Application.SYYDiagnoses.Responses;
/// <summary>SYY Tanı Listesi (M/D/F) liste/detay DTO'su, xmin sürümü ve bağlı adlar.</summary>
public sealed record SYYDiagnosisDefResponse(Guid Id,string Code,string Name,int SYYDiagnosisType,string DiagnosisTypeName,Guid? AttachedRecNo,string? AttachedDiagnosisName,bool? Eligible,int? Slice,string? SliceName,DateTime? ValidFrom,DateTime? ValidTo,bool IsActive,uint Version,DateTime CreatedDate)
{

}
