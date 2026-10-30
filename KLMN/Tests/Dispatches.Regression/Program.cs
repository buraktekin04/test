using KLMN.Application.Dispatches.Commands.SetDispatchPhoto;
using KLMN.Application.Dispatches.Commands.DeleteDispatchPhoto;
using KLMN.Application.Dispatches.Commands.CreateDispatch;
using KLMN.Application.Dispatches.Commands.UpdateDispatch;
using KLMN.Application.Dispatches.Commands.CancelDispatch;
using KLMN.Application.Dispatches.Queries.GetDispatches;
using KLMN.Application.Dispatches.Queries.GetDispatchOptions;
using KLMN.Application.Dispatches.Models;
using KLMN.Domain.Common;
using KLMN.Domain.Entities.Admissions;

static void Check(bool condition,string name)
{
    if(!condition)throw new Exception(name);
    Console.WriteLine("PASS: "+name);
}
var person=new DispatchPersonInput{Name="Örnek",Surname="Kişi",TcKimlikNo="12345678901",BirthDate=new DateOnly(1990,1,1)};
var dispatch=new DispatchDetailsInput{HcAdmissionTypeDefId=Guid.NewGuid(),
    ReasonForExaminationDefId=Guid.NewGuid(),DispatchScope=1};
Check(new CreateDispatchCommandValidator().Validate(new CreateDispatchCommand(person,dispatch)).IsValid,
    "Sevk oluşturma istek sözleşmesi");
Check(!new CreateDispatchCommandValidator().Validate(new CreateDispatchCommand(null!,dispatch)).IsValid,
    "Kişi bilgisi olmadan oluşturma engeli");
Check(!new UpdateDispatchCommandValidator().Validate(new UpdateDispatchCommand(Guid.Empty,0,person,dispatch)).IsValid,
    "Güncelleme kimliği ve versiyonu zorunlu");
Check(!new CancelDispatchCommandValidator().Validate(new CancelDispatchCommand(Guid.Empty,0)).IsValid,
    "İptal kimliği ve versiyonu zorunlu");
Check(!new GetDispatchesQueryValidator().Validate(new GetDispatchesQuery(PageSize:101)).IsValid,
    "Sevk sayfalama üst limiti");
Check(!new GetDispatchesQueryValidator().Validate(new GetDispatchesQuery(CurrentState:8)).IsValid,
    "Sevk listesinde geçersiz durum filtresi reddedilir");
Check(!new GetDispatchesQueryValidator().Validate(new GetDispatchesQuery(
    DocumentDateFrom:new DateOnly(2026,10,30),DocumentDateTo:new DateOnly(2026,10,1))).IsValid,
    "Sevk listesinde ters evrak tarihi aralığı reddedilir");
Check(new GetDispatchesQueryValidator().Validate(new GetDispatchesQuery(
    CurrentState:4,DocumentDateFrom:new DateOnly(2026,10,1),
    DocumentDateTo:new DateOnly(2026,10,30))).IsValid,
    "Sevk listesinde geçerli durum ve evrak tarihi aralığı");
Check(!new GetDispatchesQueryValidator().Validate(new GetDispatchesQuery(
    DateFrom:new DateOnly(2026,10,30),DateTo:new DateOnly(2026,10,1))).IsValid,
    "Sevk başlangıç/bitiş tarih filtrelerinde ters aralık reddi");
Check(!new GetDispatchesQueryValidator().Validate(new GetDispatchesQuery(
    DispatchNo:-1,PurchaseId:-2)).IsValid,
    "Sevk sıra no ve alım ID negatif olamaz");
Check(!new GetDispatchesQueryValidator().Validate(new GetDispatchesQuery(
    IncludeRelatedOrganizations:true)).IsValid,
    "Alt organizasyon araması için gönderen birliğin seçilmesi zorunlu");
Check(new GetDispatchesQueryValidator().Validate(new GetDispatchesQuery(
    DateFrom:new DateOnly(2026,10,1),DateTo:new DateOnly(2026,10,30),
    DispatchNo:13,PurchaseId:2,TcKimlikNo:"12345678901",
    SenderMilitaryUnitDefId:Guid.NewGuid(),IncludeRelatedOrganizations:true,
    DispatchScope:2,DispatchScopeCityId:Guid.NewGuid())).IsValid,
    "Ayrıntılı sevk sorgusu geçerli parametreleri");
Check(typeof(BaseEntity).IsAssignableFrom(typeof(DispatchStatusDef)),
    "Sevk durum tanımı BaseEntity audit/soft-delete/xmin kalıtımı");
Check(typeof(DispatchStatusDef).GetProperty(nameof(DispatchStatusDef.Code))?.PropertyType==typeof(int),
    "Sevk durumu integer DB kodu");


Check(!new GetDispatchOptionsQueryValidator().Validate(new GetDispatchOptionsQuery("bad-type")).IsValid,
    "Tanım türü allowlist dışında sorgu reddi");
Check(typeof(BaseEntity).IsAssignableFrom(typeof(Person)),"Kişi audit ve xmin kalıtımı");
Check(typeof(BaseEntity).IsAssignableFrom(typeof(Dispatch)),"Sevk audit ve xmin kalıtımı");
Check(typeof(BaseEntity).IsAssignableFrom(typeof(DispatchFile)),"Sevk dosyası audit ve xmin kalıtımı");
Check(typeof(Dispatch).GetProperty(nameof(Dispatch.HcAdmissionTypeDefId))?.PropertyType==typeof(Guid),
    "Sevk başvuru grubu FK");
Check(typeof(Dispatch).GetProperty(nameof(Dispatch.PersonId))?.PropertyType==typeof(Guid),
    "Sevk kişi FK");
Check(typeof(Dispatch).GetProperty(nameof(Dispatch.Photo))?.PropertyType==typeof(byte[]),
    "Dispatch fotoğrafı bytea/byte[] olarak korunur");
Check(typeof(Dispatch).GetProperty(nameof(Dispatch.PhotoContentType))?.PropertyType==typeof(string),
    "Fotoğraf MIME verisi kaydedilir");
var photoValidator=new SetDispatchPhotoCommandValidator();
Check(!photoValidator.Validate(new SetDispatchPhotoCommand(Guid.NewGuid(),"image/gif",[1,2,3])).IsValid,
    "İzin verilmeyen fotoğraf MIME reddi");
Check(!photoValidator.Validate(new SetDispatchPhotoCommand(Guid.NewGuid(),"image/jpeg",new byte[2097153])).IsValid,
    "2 MiB fotoğraf boyut limiti");
Check(!new DeleteDispatchPhotoCommandValidator()
    .Validate(new DeleteDispatchPhotoCommand(Guid.Empty,0)).IsValid,
    "Fotoğraf silme kimlik ve versiyon zorunluluğu");
Console.WriteLine("Not: Yerel build, PostgreSQL migration, kimlik/başvuru kuralı, izin ve dosya entegrasyon senaryoları ayrıca test edilmelidir.");
