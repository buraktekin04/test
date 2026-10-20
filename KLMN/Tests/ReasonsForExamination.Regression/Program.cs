using KLMN.Domain.Entities.Admissions;
using KLMN.Application.ReasonForExaminations.Responses;
using KLMN.Application.ReasonForExaminations.Commands.CreateReasonForExaminationDef;
using KLMN.Application.ReasonForExaminations.Queries.GetReasonForExaminationDefs;
using KLMN.Application.ReasonForExamReportTypes.Commands.CreateReasonForExamReportType;
using KLMN.Application.ReasonForExamUnusedHcAdmissionTypes.Commands.CreateReasonForExamUnusedHcAdmissionType;
using KLMN.Application.ReasonForExamProcedureDefs.Commands.CreateReasonForExamProcedureDef;
using KLMN.Application.ReasonForExamBranchDefs.Commands.CreateReasonForExamBranchDef;
using KLMN.Application.ReasonForExamBranchDefs.Commands.UpdateReasonForExamBranchDef;
using KLMN.Application.ReasonForExamBranchDefs.Responses;
using KLMN.Application.ReasonForExamProcedureDefs.Responses;
static void Check(bool ok,string title){if(!ok)throw new Exception("FAIL: "+title);Console.WriteLine("PASS: "+title);}
var reasonId=Guid.NewGuid();var targetId=Guid.NewGuid();
var reason=new ReasonForExaminationDef{Id=reasonId,Code="MUAY-01",Description="Kontrol",
 DispatchRequired=true,ClassificationExamination=false,Objectionable=true};
reason.ReportTypes.Add(new ReasonForExamReportType{ReasonForExaminationDefId=reasonId,IsActive=true,
 ReportType=new ReportTypeDef{Name="Kurul Raporu"}});
reason.UnusedAdmissionTypes.Add(new ReasonForExamUnusedHcAdmissionType{ReasonForExaminationDefId=reasonId,
 AdmissionType=new HcAdmissionTypeDef{Name="Başvuru Grubu"}});
reason.Procedures.Add(new ReasonForExamProcedureDef{ReasonForExaminationDefId=reasonId,
 Procedure=new ProcedureDef{Name="Tetkik A"},StartingAge=18,EndingAge=65,Gender=0});
reason.Branches.Add(new ReasonForExamBranchDef{ReasonForExaminationDefId=reasonId,
 Branch=new BranchDef{Name="İç Hastalıkları"},StartingAge=18,EndingAge=65,Gender=0});
var dto=ReasonForExaminationDefResponse.From(reason);
Check(dto.ReportTypes.Count==1&&dto.UnusedAdmissionTypes.Count==1&&dto.Procedures.Count==1&&dto.Branches.Count==1,
 "Ana muayene sebebi dört ilişkiyi gösterir");
Check(!new CreateReasonForExaminationDefCommandValidator().Validate(
 new CreateReasonForExaminationDefCommand("","Kontrol",true,false,false,null,null)).IsValid,"Kod zorunlu");
Check(!new GetReasonForExaminationDefsQueryValidator().Validate(new GetReasonForExaminationDefsQuery(PageSize:101)).IsValid,
 "Liste sayfa sınırı");
Check(new CreateReasonForExamReportTypeCommandValidator().Validate(new CreateReasonForExamReportTypeCommand(reasonId,targetId)).IsValid,"Rapor türü ilişki");
Check(new CreateReasonForExamUnusedHcAdmissionTypeCommandValidator().Validate(new CreateReasonForExamUnusedHcAdmissionTypeCommand(reasonId,targetId)).IsValid,
 "Kullanılmayacak hasta grubu");
Check(!new CreateReasonForExamProcedureDefCommandValidator().Validate(new CreateReasonForExamProcedureDefCommand(reasonId,targetId,60,30,0)).IsValid,
 "Ters yaş aralığı engellenir");
Check(new CreateReasonForExamBranchDefCommandValidator().Validate(new CreateReasonForExamBranchDefCommand(reasonId,targetId,18,65,0)).IsValid,
 "Muayene gereken branş");
Check(!new UpdateReasonForExamBranchDefCommandValidator().Validate(
 new UpdateReasonForExamBranchDefCommand(Guid.NewGuid(),0,reasonId,targetId,18,65,0)).IsValid,"Sıfır xmin engellenir");
var procedureLink=new ReasonForExamProcedureDef{ReasonForExaminationDefId=reasonId,Reason=reason,
 ProcedureDefId=targetId,Procedure=new ProcedureDef{Name="Tetkik A"},StartingAge=18,EndingAge=65,Gender=0};
var mapped=ReasonForExamProcedureDefResponse.Projection.Compile()(procedureLink);
Check(mapped.ReferenceName=="Tetkik A"&&mapped.EndingAge==65,"İlişki response");
Console.WriteLine("Kaynak regresyon testleri hazır; gerçek build/run ayrıca yapılmalıdır.");
