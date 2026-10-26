using KLMN.Application.HcDecisions.Commands.CreateHcDecisionDef;
using KLMN.Application.HcDecisions.Commands.UpdateHcDecisionDef;
using KLMN.Application.HcDecisions.Queries.GetHcDecisionDefs;
using KLMN.Application.HcUnusedReasons.Commands.CreateHcUnusedReasonForExam;
using KLMN.Application.HcUnusedAdmissionTypes.Commands.CreateHcUnusedAdmissionType;
using KLMN.Application.HcUnusedDecisions.Commands.CreateHcUnusedDecision;
using KLMN.Application.HcDecisionConnections.Commands.CreateHcConnectedDecision;
using KLMN.Domain.Common;
using KLMN.Domain.Entities.Admissions;
/// <summary>Karar modelleri ve komut doğrulayıcıları için hafif regresyon kontrolleri.</summary>
static void Check(bool condition,string name)
{
 if(!condition)throw new Exception(name);
 Console.WriteLine("PASS: "+name);
}
var valid=new CreateHcDecisionDefCommand(12,"Kurul Kararı",false,false,false,false,false,false,null,null);
var validator=new CreateHcDecisionDefCommandValidator();
Check(validator.Validate(valid).IsValid,"Geçerli kurul kararı");
Check(!validator.Validate(valid with{Code=-1}).IsValid,"Negatif sayısal kod");
Check(!validator.Validate(valid with{Name=""}).IsValid,"Boş karar adı");
Check(!validator.Validate(valid with{ValidFrom=DateTime.UtcNow,ValidTo=DateTime.UtcNow.AddDays(-1)}).IsValid,"Geçersiz tarih sırası");
Check(!validator.Validate(valid with{ValidFrom=DateTime.Now}).IsValid,"Geçerlilik UTC");
var edit=new UpdateHcDecisionDefCommand(Guid.Empty,0,12,"A",false,false,false,false,false,false,null,null);
Check(!new UpdateHcDecisionDefCommandValidator().Validate(edit).IsValid,"Güncelleme Id zorunlu");
Check(!new GetHcDecisionDefsQueryValidator().Validate(new GetHcDecisionDefsQuery(PageSize:101)).IsValid,"Sayfa boyutu sınırı");
Check(typeof(EffectiveDatedEntity).IsAssignableFrom(typeof(HcDecisionDef)),"Kararlar ortak tarih mirasını kullanır");
Check(typeof(BaseEntity).IsAssignableFrom(typeof(HcUnusedDecision)),"İlişkiler BaseEntity kullanır");
Check(typeof(HcDecisionDef).GetProperty(nameof(HcDecisionDef.ValidFrom))?.DeclaringType==typeof(EffectiveDatedEntity),"ValidFrom yeniden tanımlanmaz");
Check(!new CreateHcUnusedReasonForExamCommandValidator().Validate(new(Guid.Empty,Guid.NewGuid())).IsValid,"Muayene sebebi ilişkisinde karar zorunlu");
Check(!new CreateHcUnusedAdmissionTypeCommandValidator().Validate(new(Guid.NewGuid(),Guid.Empty)).IsValid,"Başvuru grubu FK zorunlu");
Check(!new CreateHcUnusedDecisionCommandValidator().Validate(new(Guid.Empty,Guid.Empty)).IsValid,"Olumsuz karar FK zorunlu");
Check(!new CreateHcConnectedDecisionCommandValidator().Validate(new(Guid.NewGuid(),Guid.Empty)).IsValid,"Bağlı karar FK zorunlu");
Console.WriteLine("KLMN kurul karar regresyon statik doğrulamaları tamamlandı.");
