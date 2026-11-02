using KLMN.Application.SYYRules.Queries.GetSYYRules;
using KLMN.Application.SYYDiagnosisRequireds.Commands.CreateSYYDiagnosisRequired;
using KLMN.Application.SYYDiagnosisRequireds.Commands.UpdateSYYDiagnosisRequired;
using KLMN.Application.SYYRules.Responses;
using KLMN.Domain.Entities.Admissions;

static void Check(bool condition,string name)
{
 if(!condition)throw new Exception(name);
 Console.WriteLine("PASS "+name);
}
var validator=new GetSYYRulesQueryValidator();
Check(validator.Validate(new GetSYYRulesQuery()).IsValid,"Varsayılan okuma sorgusu");
Check(!validator.Validate(new GetSYYRulesQuery(PageNumber:0)).IsValid,"Sayfa sıfır engeli");
Check(!validator.Validate(new GetSYYRulesQuery(PageSize:101)).IsValid,"Sayfa boyutu sınırı");
Check(!validator.Validate(new GetSYYRulesQuery(Search:new string('x',301))).IsValid,"Arama uzunluğu sınırı");
Check(new GetSYYRulesQuery(IncludeInactive:true).IncludeInactive,"Pasif kayıtları getirme seçeneği");
Check(typeof(SYYDiagnosisDef).GetProperty(nameof(SYYDiagnosisDef.Eligible))?.PropertyType==typeof(bool?),
  "Yeni bir entity yerine mevcut nullable Eligible kullanılır");
var rule=new SYYRuleResponse(Guid.NewGuid(),"SYY-1","Tanı","Tür",null,null,null,true,
 [new SYYRuleBranchResponse(Guid.NewGuid(),"Branş",0,130,0,true)],
 [new SYYRuleProcedureResponse(Guid.NewGuid(),"Tetkik",0,130,0,true)],
 [new SYYRuleApprovalResponse(Guid.NewGuid(),true,false,true,
   [new SYYRuleApprovalDetailResponse(Guid.NewGuid(),"Açıklama",1)])]);
Check(rule.Branches.Count==1&&rule.Procedures.Count==1&&rule.Approvals.Count==1,
 "Üç mevcut eşleştirme veri sözleşmesi");
Check(rule.Approvals[0].Details.Count==1,"Onay açıklama maddeleri");
// Onay kuralı: tam olarak bir koşul seçilmeli (ikisi birden veya hiçbiri olamaz).
var createRequiredValidator=new CreateSYYDiagnosisRequiredCommandValidator();
var updateRequiredValidator=new UpdateSYYDiagnosisRequiredCommandValidator();
foreach(var (all,one,isAllowed) in new (bool?,bool?,bool)[]{
    (true,false,true),(false,true,true),
    (true,true,false),(false,false,false),
    (null,null,false),(true,null,false),(null,true,false),
    (null,false,false),(false,null,false)
})
{
    var create=new CreateSYYDiagnosisRequiredCommand(Guid.NewGuid(),all,one,["Madde 1"]);
    var update=new UpdateSYYDiagnosisRequiredCommand(Guid.NewGuid(),1u,Guid.NewGuid(),all,one,["Madde 1"]);
    Check(createRequiredValidator.Validate(create).IsValid==isAllowed,
       $"Create radio koşulu MustAll={all?.ToString()??"null"} MustOneOf={one?.ToString()??"null"}");
    Check(updateRequiredValidator.Validate(update).IsValid==isAllowed,
       $"Update radio koşulu MustAll={all?.ToString()??"null"} MustOneOf={one?.ToString()??"null"}");
}
Console.WriteLine("Bu kontroller sadece model/validator regresyonudur; DB filtre testi ayrıca çalıştırılmalıdır.");
