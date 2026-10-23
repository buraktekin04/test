using KLMN.Domain.Entities.Admissions;
namespace KLMN.Application.SYYDiagnosisProcedures.Responses;
/// <summary>SYY Gereken Tetkikler: ilişkilerin kod/isimlerini ve xmin sürümünü taşıyan DTO.</summary>
public sealed record SYYDiagnosisProcedureDefResponse(Guid Id,Guid SYYDiagnosisDefId,Guid ProcedureDefId,string DiagnosisName,string ProcedureName,int StartingAge,int EndingAge,int Gender,bool IsActive,uint Version,DateTime CreatedDate)
{

}
