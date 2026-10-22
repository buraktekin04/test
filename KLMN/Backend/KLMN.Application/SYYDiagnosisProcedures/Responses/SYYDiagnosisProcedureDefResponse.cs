using System.Linq.Expressions;
using KLMN.Domain.Entities.Admissions;
namespace KLMN.Application.SYYDiagnosisProcedures.Responses;
/// <summary>SYY Gereken Tetkikler: ilişkilerin kod/isimlerini ve xmin sürümünü taşıyan DTO.</summary>
public sealed record SYYDiagnosisProcedureDefResponse(Guid Id,Guid SYYDiagnosisDefId,Guid ProcedureDefId,string DiagnosisName,string ProcedureName,int StartingAge,int EndingAge,int Gender,bool IsActive,uint Version,DateTime CreatedDate)
{
 public static Expression<Func<SYYDiagnosisProcedureDef,SYYDiagnosisProcedureDefResponse>> Projection=>x=>new(x.Id,x.SYYDiagnosisDefId,x.ProcedureDefId,x.Diagnosis.Name,x.Procedure.Name,x.StartingAge,x.EndingAge,x.Gender,x.IsActive,x.Version,x.CreatedDate);
}
public sealed record SYYDiagnosisProcedureDefMutationResponse(Guid Id,uint Version);
