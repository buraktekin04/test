using System.Linq.Expressions;
using KLMN.Domain.Entities.Admissions;
namespace KLMN.Application.SYYDiagnosisBranches.Responses;
/// <summary>SYY Gereken Branşlar: ilişkilerin kod/isimlerini ve xmin sürümünü taşıyan DTO.</summary>
public sealed record SYYDiagnosisBranchDefResponse(Guid Id,Guid SYYDiagnosisDefId,Guid BranchDefId,string DiagnosisName,string BranchName,int StartingAge,int EndingAge,int Gender,bool IsActive,uint Version,DateTime CreatedDate)
{
 public static Expression<Func<SYYDiagnosisBranchDef,SYYDiagnosisBranchDefResponse>> Projection=>x=>new(x.Id,x.SYYDiagnosisDefId,x.BranchDefId,x.Diagnosis.Name,x.Branch.Name,x.StartingAge,x.EndingAge,x.Gender,x.IsActive,x.Version,x.CreatedDate);
}
public sealed record SYYDiagnosisBranchDefMutationResponse(Guid Id,uint Version);
