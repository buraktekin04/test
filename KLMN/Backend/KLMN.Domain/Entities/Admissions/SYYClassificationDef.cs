using KLMN.Domain.Common;
using KLMN.Domain.Entities.MilitaryUnit;
namespace KLMN.Domain.Entities.Admissions;
/// <summary>SYY Tanı Sınıflandırma Çizelgesi Ekranı — mevcut tanım kayıtlarına bağlı tarihli sınıflandırma.</summary>
public sealed class SYYClassificationDef:EffectiveDatedEntity
{
 public Guid SYYDiagnosisDefId {get;set;}
 public SYYDiagnosisDef Diagnosis {get;set;}=null!;
 public Guid ForcesCommandDef {get;set;}
 public ForcesCommandDef ForcesCommand {get;set;}=null!;
 public Guid RankGroup {get;set;}
 public RankGroupDef RankGroupDefinition {get;set;}=null!;
 public Guid MilitaryClassGroup {get;set;}
 public MilitaryClassGroupDef MilitaryClassGroupDefinition {get;set;}=null!;
 public Guid HcDecision {get;set;}
 public HcConnectedDecisionDef Decision {get;set;}=null!;
}
