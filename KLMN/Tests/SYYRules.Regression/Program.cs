using KLMN.Application.SYYRules.Queries.GetSYYRules;
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
Console.WriteLine("Bu kontroller sadece model/validator regresyonudur; DB filtre testi ayrıca çalıştırılmalıdır.");
