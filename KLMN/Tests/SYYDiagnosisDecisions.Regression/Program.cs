using KLMN.Application.SYYDiagnosisDecisions.Commands.CreateSYYDiagnosisDecision;
using KLMN.Application.SYYDiagnosisDecisions.Commands.UpdateSYYDiagnosisDecision;
using KLMN.Application.SYYDiagnosisDecisions.Queries.GetSYYDiagnosisDecisions;
using KLMN.Application.SYYDiagnosisDecisions.Queries.GetSYYDiagnosisDecisionOptions;
using KLMN.Domain.Common;
using KLMN.Domain.Entities.Admissions;
static void Check(bool condition,string text){
 if(!condition)throw new Exception(text);Console.WriteLine("PASS: "+text);
}
var syy=Guid.NewGuid();var decision=Guid.NewGuid();var admission=Guid.NewGuid();var military=Guid.NewGuid();
var valid=new CreateSYYDiagnosisDecisionCommand(syy,decision,admission,null,0);
var validator=new CreateSYYDiagnosisDecisionCommandValidator();
Check(validator.Validate(valid).IsValid,"Askerî sınıf boş, adet sıfır");
Check(validator.Validate(valid with{MilitaryClassDefId=military,Amount=3}).IsValid,"Askerî sınıf seçimi");
Check(!validator.Validate(valid with{Amount=-1}).IsValid,"Negatif adet engeli");
Check(!validator.Validate(valid with{SYYDiagnosisDefId=Guid.Empty}).IsValid,"Tanı zorunlu");
Check(!validator.Validate(valid with{HcDecisionDefId=Guid.Empty}).IsValid,"Kurul kararı zorunlu");
Check(!validator.Validate(valid with{HcAdmissionTypeDefId=Guid.Empty}).IsValid,"Başvuru grubu zorunlu");
var update=new UpdateSYYDiagnosisDecisionCommand(Guid.Empty,0,syy,decision,admission,null,0);
Check(!new UpdateSYYDiagnosisDecisionCommandValidator().Validate(update).IsValid,"Update Id zorunlu");
Check(!new GetSYYDiagnosisDecisionsQueryValidator().Validate(new GetSYYDiagnosisDecisionsQuery(PageSize:101)).IsValid,"Sayfa boyutu üst sınırı");
var filter=new GetSYYDiagnosisDecisionsQuery(SYYDiagnosisDefId:syy,HcAdmissionTypeDefId:admission,MilitaryClassDefId:military);
Check(filter.SYYDiagnosisDefId==syy&&filter.HcAdmissionTypeDefId==admission&&filter.MilitaryClassDefId==military,
  "Üç bağımsız dropdown filtresi sorguya aktarılır");
Check(typeof(BaseEntity).IsAssignableFrom(typeof(SYYDiagnosisDecision)),"BaseEntity kalıtımı");
Check(new GetSYYDiagnosisDecisionOptionsQuery().SelectedMilitaryClassDefId==null,"Nullable askerî sınıf seçeneği");
Console.WriteLine("SYY Tanı Karar İstatistiği regresyon doğrulamaları tamamlandı.");
