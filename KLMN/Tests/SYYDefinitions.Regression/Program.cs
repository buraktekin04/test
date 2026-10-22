using KLMN.Application.SYYClassifications.Commands.CreateSYYClassificationDef;
using KLMN.Application.SYYClassifications.Queries.GetSYYClassificationDefs;
using KLMN.Application.SYYDiagnosisBranches.Commands.CreateSYYDiagnosisBranchDef;
using KLMN.Application.SYYDiagnosisProcedures.Commands.CreateSYYDiagnosisProcedureDef;
using KLMN.Application.SYYDiagnosisRequireds.Commands.CreateSYYDiagnosisRequired;
using KLMN.Application.SYYDiagnosisRequireds.Commands.UpdateSYYDiagnosisRequired;
using KLMN.Domain.Common;
using KLMN.Domain.Entities.Admissions;
using KLMN.Application.SYYDiagnoses.Commands.CreateSYYDiagnosisDef;
using KLMN.Application.SYYDiagnoses.Commands.UpdateSYYDiagnosisDef;
using KLMN.Application.SYYDiagnoses.Queries.GetSYYDiagnosisDefs;
using KLMN.Application.SYYDiagnoses.Responses;
using KLMN.Application.SYYDiagnosisTypes.Commands.CreateSYYDiagnosisTypeDef;
using KLMN.Application.SYYDiagnosisSlices.Commands.CreateSYYDiagnosisSliceDef;
using KLMN.Application.AnnotationReasons.Commands.CreateAnnotationReasonDef;
using KLMN.Application.PlaneTypes.Commands.CreatePlaneTypeDef;
using KLMN.Application.FlyingMissions.Commands.CreateFlyingMissionDef;
static void Check(bool ok,string title){if(!ok)throw new Exception("FAIL: "+title);Console.WriteLine("PASS: "+title);}
var id=Guid.NewGuid();
Check(typeof(SYYDiagnosisDef).BaseType==typeof(EffectiveDatedEntity),"SYY tanı tarihleri EffectiveDatedEntity");
Check(typeof(AnnotationReasonDef).BaseType==typeof(EffectiveDatedEntity),"Sevk Sebebi ortak tarihleri");
Check(typeof(PlaneTypeDef).BaseType==typeof(EffectiveDatedEntity),"Uçak Tipi ortak tarihleri");
Check(typeof(FlyingMissionDef).BaseType==typeof(EffectiveDatedEntity),"Uçucu Görev ortak tarihleri");
Check(typeof(SYYDiagnosisTypeDef).BaseType==typeof(BaseEntity),"Tanı Türü yalnız BaseEntity");
Check(typeof(SYYDiagnosisSliceDef).BaseType==typeof(BaseEntity),"Dilim yalnız BaseEntity");
Check(typeof(SYYDiagnosisDef).GetProperty(nameof(SYYDiagnosisDef.AttachedRecNo))!.PropertyType==typeof(Guid?),"Self FK nullable GUID");
Check(typeof(SYYDiagnosisDef).GetProperty(nameof(SYYDiagnosisDef.Eligible))!.PropertyType==typeof(bool?),"Eligible nullable bool korunur");
Check(typeof(SYYDiagnosisDef).GetProperty(nameof(SYYDiagnosisDef.Slice))!.PropertyType==typeof(int?),"Dilim nullable int korunur");
var from=new DateTime(2026,10,22,9,0,0,DateTimeKind.Utc);
var cmd=new CreateSYYDiagnosisDefCommand("M.1","Tanı",1,null,true,2,from,from.AddMonths(1));
Check(new CreateSYYDiagnosisDefCommandValidator().Validate(cmd).IsValid,"SYY tanısı geçerli");
Check(!new CreateSYYDiagnosisDefCommandValidator().Validate(cmd with{Code=""}).IsValid,"SYY kod zorunluluğu");
Check(!new CreateSYYDiagnosisDefCommandValidator().Validate(cmd with{ValidTo=from}).IsValid,"Geçerlilik aralığı");
Check(!new UpdateSYYDiagnosisDefCommandValidator().Validate(
 new UpdateSYYDiagnosisDefCommand(id,0,"M.1","Tanı",1,null,null,3,from,null)).IsValid,"xmin zorunlu");
Check(!new GetSYYDiagnosisDefsQueryValidator().Validate(new(PageSize:101)).IsValid,"Backend sayfa limiti");
Check(new CreateSYYDiagnosisSliceDefCommandValidator().Validate(new(1,"A")).IsValid,"Dilim 1 A geçerli");
Check(new CreateSYYDiagnosisSliceDefCommandValidator().Validate(new(4,"D")).IsValid,"Dilim 4 D geçerli");
Check(!new CreateSYYDiagnosisTypeDefCommandValidator().Validate(new(-1,"Tanı türü")).IsValid,"Negatif tanı türü engeli");
Check(new CreateAnnotationReasonDefCommandValidator().Validate(new("S1","Sevk",null,null)).IsValid,"Sevk sebebi tanımı");
Check(new CreatePlaneTypeDefCommandValidator().Validate(new("P1","Uçak",null,null)).IsValid,"Uçak tipi tanımı");
Check(new CreateFlyingMissionDefCommandValidator().Validate(new("G1","Görev",null,null)).IsValid,"Uçucu görev tanımı");
var type=new SYYDiagnosisTypeDef{Code=1,Name="Tür"};
var slice=new SYYDiagnosisSliceDef{Code=2,Name="B"};
var parent=new SYYDiagnosisDef{Id=id,Code="M",Name="Ana tanı"};
var diagnosis=new SYYDiagnosisDef{Code="M.1",Name="Tanı",SYYDiagnosisType=1,DiagnosisTypeDef=type,
 Slice=2,SliceDef=slice,Eligible=null,AttachedRecNo=id,AttachedDiagnosis=parent,ValidFrom=from};
var dto=SYYDiagnosisDefResponse.Projection.Compile()(diagnosis);
Check(dto.DiagnosisTypeName=="Tür"&&dto.SliceName=="B"&&dto.AttachedDiagnosisName=="Ana tanı","Bağlı kayıt adları");
Check(dto.Eligible==null&&dto.ValidFrom==from,"Nullable/ValidFrom korunur");

Check(typeof(SYYClassificationDef).BaseType==typeof(EffectiveDatedEntity),
 "SYY Tanı Sınıflandırma Çizelgesi EffectiveDatedEntity kalıtımı");
Check(typeof(SYYDiagnosisBranchDef).BaseType==typeof(BaseEntity),"SYY branş ilişki BaseEntity");
Check(typeof(SYYDiagnosisProcedureDef).BaseType==typeof(BaseEntity),"SYY tetkik ilişki BaseEntity");
Check(typeof(SYYDiagnosisRequired).BaseType==typeof(BaseEntity),"SYY onay kuralı BaseEntity");
Check(typeof(SYYDiagnosisRequiredDetail).BaseType==typeof(BaseEntity),"SYY kural maddeleri BaseEntity");
var force=Guid.NewGuid();var rank=Guid.NewGuid();var group=Guid.NewGuid();var decision=Guid.NewGuid();
var classification=new CreateSYYClassificationDefCommand(id,force,rank,group,decision,from,from.AddYears(1));
Check(new CreateSYYClassificationDefCommandValidator().Validate(classification).IsValid,
 "SYY çizelgesi 5 mevcut FK ve ValidFrom/ValidTo ile geçerli");
Check(!new CreateSYYClassificationDefCommandValidator().Validate(classification with{ValidTo=from}).IsValid,
 "SYY çizelgesi geçerlilik aralığı korunur");
Check(new CreateSYYDiagnosisBranchDefCommandValidator().Validate(
 new(id,Guid.NewGuid(),18,65,0)).IsValid,"SYY branş yaş aralığı");
Check(!new CreateSYYDiagnosisBranchDefCommandValidator().Validate(
 new(id,Guid.NewGuid(),65,18,0)).IsValid,"SYY branş ters yaş aralığı engellenir");
Check(new CreateSYYDiagnosisProcedureDefCommandValidator().Validate(
 new(id,Guid.NewGuid(),18,65,0)).IsValid,"SYY tetkik yaş aralığı");
Check(!new CreateSYYDiagnosisProcedureDefCommandValidator().Validate(
 new(id,Guid.NewGuid(),0,131,0)).IsValid,"SYY tetkik 130 üzeri yaş engellenir");
var mustAll=new CreateSYYDiagnosisRequiredCommand(id,true,false,new[]{"1. madde","2. madde"});
Check(new CreateSYYDiagnosisRequiredCommandValidator().Validate(mustAll).IsValid,"SYY kurallı maddeler");
Check(!new CreateSYYDiagnosisRequiredCommandValidator().Validate(
 mustAll with{MustOneOf=true}).IsValid,"MustAll ve MustOneOf birlikte seçilmez");
Check(!new CreateSYYDiagnosisRequiredCommandValidator().Validate(
 mustAll with{Descriptions=Array.Empty<string>()}).IsValid,"Boş onay açıklaması engellenir");
Check(!new UpdateSYYDiagnosisRequiredCommandValidator().Validate(
 new(Guid.NewGuid(),0,id,true,false,new[]{"Madde"})).IsValid,"Kural değişiminde xmin zorunlu");
Check(!new GetSYYClassificationDefsQueryValidator().Validate(new(PageSize:101)).IsValid,
 "SYY çizelgesi sayfalama üst sınırı");

Console.WriteLine("Yalnız kaynak regresyonu; gerçek DB, cycle kontrolü, migration ve tarayıcı testi ayrıca yapılır.");
