using KLMN.Application.Provisions.Commands.CreateWithDispatch;
using KLMN.Application.Provisions.Commands.CreateWithoutDispatch;
using KLMN.Application.Provisions.Commands.CancelProvision;
using KLMN.Application.Provisions.Queries.GetProvisions;
using KLMN.Application.Dispatches.Models;
using KLMN.Domain.Common;
using KLMN.Domain.Entities.Admissions;

static void Check(bool value,string name)
{
    if(!value)throw new Exception("FAIL: "+name);
    Console.WriteLine("PASS: "+name);
}
var dispatchId=Guid.Parse("08f302c0-a341-48da-9811-5884ef000001");
var hospitalId=Guid.Parse("08f302c0-a341-48da-9811-5884ef000002");
var withV=new CreateWithDispatchCommandValidator();
Check(withV.Validate(new CreateWithDispatchCommand(dispatchId,hospitalId)).IsValid,
    "Sevkli provizyon için aynı mevcut sevk GUID'si");
Check(withV.Validate(new CreateWithDispatchCommand(null,hospitalId,112L)).IsValid,
    "Sevkli provizyon sevk sıra numarasıyla");
Check(!withV.Validate(new CreateWithDispatchCommand(null,hospitalId)).IsValid,
    "Sevk olmadan sevkli provizyon oluşturulmaz");
Check(!withV.Validate(new CreateWithDispatchCommand(dispatchId,hospitalId,12L)).IsValid,
    "Sevk GUID ve sıra numarası birlikte alınmaz");
Check(!withV.Validate(new CreateWithDispatchCommand(dispatchId,Guid.Empty)).IsValid,
    "Kabul yapan hastane GUID'si zorunlu");
var withoutV=new CreateWithoutDispatchCommandValidator();
var p=new DispatchPersonInput {TcKimlikNo="12345678901",Name="Örnek",
    Surname="Kişi",BirthDate=new DateOnly(1990,1,1)};
var d=new DispatchDetailsInput {
    HcAdmissionTypeDefId=dispatchId,ReasonForExaminationDefId=hospitalId,
    SenderMilitaryUnitDefId=dispatchId
};
Check(withoutV.Validate(new CreateWithoutDispatchCommand(hospitalId,p,d)).IsValid,
    "Sevksiz provizyon kişi, askerî veri, kabul hastanesi");
Check(!withoutV.Validate(new CreateWithoutDispatchCommand(Guid.Empty,p,d)).IsValid,
    "Sevksiz provizyonda hastane zorunlu");
Check(!withoutV.Validate(new CreateWithoutDispatchCommand(hospitalId,p,null!)).IsValid,
    "Sevksiz provizyonda sevk/başvuru detayı zorunlu");
var cancelV=new CancelProvisionCommandValidator();
Check(cancelV.Validate(new CancelProvisionCommand(dispatchId,1,"İşlem iptal açıklaması")).IsValid,
    "Provizyon iptalinde açıklama ve xmin");
Check(!cancelV.Validate(new CancelProvisionCommand(dispatchId,1,"")).IsValid,
    "İptal açıklaması zorunlu");
Check(!cancelV.Validate(new CancelProvisionCommand(dispatchId,0,"Açıklama")).IsValid,
    "İptal için geçerli xmin sürümü gerekir");
var queryV=new GetProvisionsQueryValidator();
Check(queryV.Validate(new GetProvisionsQuery(PageNumber:1,PageSize:20)).IsValid,
    "Geçerli provizyon sorgu sayfalama");
Check(!queryV.Validate(new GetProvisionsQuery(PageNumber:0,PageSize:101)).IsValid,
    "Geçersiz sayfalama reddedilir");
Check(!queryV.Validate(new GetProvisionsQuery(CurrentState:5)).IsValid,
    "Eski 1-4 durum enum dışı sorgulanamaz");
Check(typeof(BaseEntity).IsAssignableFrom(typeof(Provision)),"Provision audit ve xmin ortak entity");
Check(typeof(BaseEntity).IsAssignableFrom(typeof(ProvisionState)),"ProvisionState audit ve xmin ortak entity");
Check(typeof(Provision).GetProperty(nameof(Provision.DispatchId))?.PropertyType==typeof(Guid),
    "Sevk GUID aynı kalır");
Check(typeof(Provision).GetProperty(nameof(Provision.DispatchHospitalId))?.PropertyType==typeof(Guid),
    "Hastane GUID aynı kalır");
Check(typeof(ProvisionState).GetProperty(nameof(ProvisionState.ProvisionId))?.PropertyType==typeof(Guid),
    "ProvisionState eski provizyon GUID ile ilişki");
Console.WriteLine("Not: Gerçek EF Core migration, PostgreSQL ve bakanlık servis testleri ayrıca yapılmalıdır.");
