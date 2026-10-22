using System.Linq.Expressions;
using KLMN.Domain.Entities.Admissions;
namespace KLMN.Application.SYYClassifications.Responses;
/// <summary>SYY Tanı Sınıflandırma Çizelgesi: ilişkilerin kod/isimlerini ve xmin sürümünü taşıyan DTO.</summary>
public sealed record SYYClassificationDefResponse(Guid Id,Guid SYYDiagnosisDefId,Guid ForcesCommandDef,Guid RankGroup,Guid MilitaryClassGroup,Guid HcDecision,string DiagnosisName,string ForcesCommandName,string RankGroupDefinitionName,string MilitaryClassGroupDefinitionName,string DecisionName,DateTime? ValidFrom,DateTime? ValidTo,bool IsActive,uint Version,DateTime CreatedDate)
{
 public static Expression<Func<SYYClassificationDef,SYYClassificationDefResponse>> Projection=>x=>new(x.Id,x.SYYDiagnosisDefId,x.ForcesCommandDef,x.RankGroup,x.MilitaryClassGroup,x.HcDecision,x.Diagnosis.Name,x.ForcesCommand.Name,x.RankGroupDefinition.Name,x.MilitaryClassGroupDefinition.Name,x.Decision.Name,x.ValidFrom,x.ValidTo,x.IsActive,x.Version,x.CreatedDate);
}
public sealed record SYYClassificationDefMutationResponse(Guid Id,uint Version);
