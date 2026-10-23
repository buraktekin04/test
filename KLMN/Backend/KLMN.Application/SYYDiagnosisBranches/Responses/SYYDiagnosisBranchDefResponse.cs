using KLMN.Domain.Entities.Admissions;
namespace KLMN.Application.SYYDiagnosisBranches.Responses;
/// <summary>SYY Gereken Branşlar: ilişkilerin kod/isimlerini ve xmin sürümünü taşıyan DTO.</summary>
public sealed record SYYDiagnosisBranchDefResponse(Guid Id,Guid SYYDiagnosisDefId,Guid BranchDefId,string DiagnosisName,string BranchName,int StartingAge,int EndingAge,int Gender,bool IsActive,uint Version,DateTime CreatedDate)
{

}
