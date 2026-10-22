using System.Linq.Expressions;
using KLMN.Domain.Entities.Admissions;
namespace KLMN.Application.SYYDiagnoses.Responses;
/// <summary>SYY Tanı Listesi (M/D/F) liste/detay DTO'su, xmin sürümü ve bağlı adlar.</summary>
public sealed record SYYDiagnosisDefResponse(Guid Id,string Code,string Name,int SYYDiagnosisType,string DiagnosisTypeName,Guid? AttachedRecNo,string? AttachedDiagnosisName,bool? Eligible,int? Slice,string? SliceName,DateTime? ValidFrom,DateTime? ValidTo,bool IsActive,uint Version,DateTime CreatedDate)
{
 public static Expression<Func<SYYDiagnosisDef,SYYDiagnosisDefResponse>> Projection=>x=>new(
 x.Id,x.Code,x.Name,x.SYYDiagnosisType,x.DiagnosisTypeDef.Name,x.AttachedRecNo,x.AttachedDiagnosis!=null?x.AttachedDiagnosis.Name:null,
 x.Eligible,x.Slice,x.SliceDef!=null?x.SliceDef.Name:null,x.ValidFrom,x.ValidTo,x.IsActive,x.Version,x.CreatedDate);
}
/// <summary>Kaydedilen nesnenin Id ve xmin sürümü.</summary>
public sealed record SYYDiagnosisDefMutationResponse(Guid Id,uint Version);
