using KLMN.Domain.Entities.Admissions;
namespace KLMN.Application.SYYClassifications.Responses;
/// <summary>SYY Tanı Sınıflandırma Çizelgesi: ilişkilerin kod/isimlerini ve xmin sürümünü taşıyan DTO.</summary>
public sealed record SYYClassificationDefResponse(Guid Id,Guid SYYDiagnosisDefId,Guid ForcesCommandDef,Guid RankGroup,Guid MilitaryClassGroup,Guid HcDecision,string DiagnosisName,string ForcesCommandName,string RankGroupDefinitionName,string MilitaryClassGroupDefinitionName,string DecisionName,DateTime? ValidFrom,DateTime? ValidTo,bool IsActive,uint Version,DateTime CreatedDate)
{

}
