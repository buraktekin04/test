using KLMN.Application.Dispatches.Queries.LookupDispatchPerson;
using KLMN.Application.Dispatches.Exceptions;
using KLMN.Application.Dispatches.Commands.SetDispatchPhoto;
using KLMN.Application.Dispatches.Commands.DeleteDispatchPhoto;
using KLMN.Application.Dispatches.Commands.CreateDispatch;
using KLMN.Application.Dispatches.Commands.CreateBulkDispatch;
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
static DispatchDetailsInput Scope(int scope,Guid? city=null,Guid? hospital=null)
{
    return new DispatchDetailsInput
    {
        HcAdmissionTypeDefId=Guid.NewGuid(),
        ReasonForExaminationDefId=Guid.NewGuid(),
        DispatchScope=scope,DispatchScopeCityId=city,DispatchScopeHospitalId=hospital
    };
}
var dispatch=Scope(2,city:Guid.NewGuid());
var createValidator=new CreateDispatchCommandValidator();
var updateValidator=new UpdateDispatchCommandValidator();
Check(createValidator.Validate(new CreateDispatchCommand(person,dispatch)).IsValid,
    "Sevk oluşturma istek sözleşmesi (şehir kapsamı)");
Check(createValidator.Validate(new CreateDispatchCommand(person,Scope(3,hospital:Guid.NewGuid()))).IsValid,
    "Sevk oluşturma belirli sağlık tesisi kapsamı");
Check(!createValidator.Validate(new CreateDispatchCommand(person,new DispatchDetailsInput())).IsValid,
    "Sevk oluştururken kapsam gönderilmemesi reddedilir");
Check(!createValidator.Validate(new CreateDispatchCommand(person,Scope(1))).IsValid,
    "Sevk oluştururken kapsam belirtilmemiş (1) reddedilir");
Check(!createValidator.Validate(new CreateDispatchCommand(person,Scope(2))).IsValid,
    "Şehir kapsamı seçildiğinde şehir zorunlu");
Check(!createValidator.Validate(new CreateDispatchCommand(person,Scope(3))).IsValid,
    "Sağlık tesisi kapsamı seçildiğinde hastane zorunlu");
Check(!updateValidator.Validate(new UpdateDispatchCommand(Guid.NewGuid(),1,person,Scope(1))).IsValid,
    "Sevk düzenlerken kapsam belirtilmemiş (1) reddedilir");
Check(!new CreateDispatchCommandValidator().Validate(new CreateDispatchCommand(null!,dispatch)).IsValid,
    "Kişi bilgisi olmadan oluşturma engeli");
var bulkValidator=new CreateBulkDispatchCommandValidator();
var bulkRow=new BulkDispatchItemInput { Person=person,Dispatch=dispatch };
Check(bulkValidator.Validate(new CreateBulkDispatchCommand([bulkRow])).IsValid,
    "Toplu sevkte tek geçerli satır");
Check(!bulkValidator.Validate(new CreateBulkDispatchCommand([])).IsValid,
    "Toplu sevkte boş dosya reddedilir");
Check(!bulkValidator.Validate(new CreateBulkDispatchCommand(
    Enumerable.Repeat(bulkRow,101).ToArray())).IsValid,
    "Toplu sevkte 100 kayıt sınırı");
Check(!bulkValidator.Validate(new CreateBulkDispatchCommand([
    new BulkDispatchItemInput { Person=null!,Dispatch=dispatch }
])).IsValid,"Toplu sevkte kişisiz kayıt reddedilir");
Check(!bulkValidator.Validate(new CreateBulkDispatchCommand([
    new BulkDispatchItemInput { Person=person,Dispatch=null! }
])).IsValid,"Toplu sevkte sevk detayı olmayan kayıt reddedilir");
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
var scopeQueryValidator=new GetDispatchesQueryValidator();
Check(scopeQueryValidator.Validate(new GetDispatchesQuery(DispatchScope:null)).IsValid,
    "Sevk kapsamı filtresi boş bırakıldığında tüm kayıtlar (tarihsel kapsam=1 dahil) sorgulanabilir");
Check(scopeQueryValidator.Validate(new GetDispatchesQuery(DispatchScope:2)).IsValid,
    "Sevk kapsamına göre Şehirdeki sağlık tesisleri filtrelenebilir");
Check(scopeQueryValidator.Validate(new GetDispatchesQuery(DispatchScope:3)).IsValid,
    "Sevk kapsamına göre Belirli sağlık tesisi filtrelenebilir");
Check(!scopeQueryValidator.Validate(new GetDispatchesQuery(DispatchScope:1)).IsValid,
    "Sevk sorgusunda Kapsam belirtilmemiş seçeneği reddedilir");
Check(!scopeQueryValidator.Validate(new GetDispatchesQuery(DispatchScope:0)).IsValid,
    "Sevk sorgusunda geçersiz kapsam reddedilir");
Check(typeof(BaseEntity).IsAssignableFrom(typeof(GenderDef)),
    "Cinsiyet tanımları BaseEntity audit/xmin kalıtımı");
Check(typeof(GenderDef).GetProperty(nameof(GenderDef.Code))?.PropertyType==typeof(int),
    "Cinsiyet enum uyumlu kodları DB sözlüğündedir");
Check(typeof(BaseEntity).IsAssignableFrom(typeof(DispatchStatusDef)),
    "Sevk durum tanımı BaseEntity audit/soft-delete/xmin kalıtımı");
Check(typeof(DispatchStatusDef).GetProperty(nameof(DispatchStatusDef.Code))?.PropertyType==typeof(int),
    "Sevk durumu integer DB kodu");


var dispatchOptionValidator=new GetDispatchOptionsQueryValidator();
Check(dispatchOptionValidator.Validate(new GetDispatchOptionsQuery("senders")).IsValid,
    "Muayeneye Gönderen Makam (senders) kabul edilir");
Check(dispatchOptionValidator.Validate(new GetDispatchOptionsQuery("admissionTypes")).IsValid,
    "Başvuru Grubu (admissionTypes) kabul edilir");
Check(!dispatchOptionValidator.Validate(new GetDispatchOptionsQuery("bad-type")).IsValid,
    "Tanım türü allowlist dışında sorgu reddi");
var registryValidator=new LookupDispatchPersonQueryValidator();
Check(registryValidator.Validate(new LookupDispatchPersonQuery("12345678901",null,new DateOnly(1990,1,1))).IsValid,
    "Nüfus sorgusu geçerli kimlik ve doğum tarihi");
Check(!registryValidator.Validate(new LookupDispatchPersonQuery(null,null,new DateOnly(1990,1,1))).IsValid,
    "Nüfus sorgusunda kimlik alanı zorunlu");
Check(!registryValidator.Validate(new LookupDispatchPersonQuery("12345678901","12345678901",new DateOnly(1990,1,1))).IsValid,
    "Nüfus sorgusunda iki kimlik türü birlikte gönderilemez");
var unavailable=false;
try
{
    await new LookupDispatchPersonQueryHandler().Handle(
        new LookupDispatchPersonQuery("12345678901",null,new DateOnly(1990,1,1)),CancellationToken.None);
}
catch(NationalRegistryNotConfiguredException)
{
    unavailable=true;
}
Check(unavailable,"Bağlanmamış nüfus entegrasyonu sahte/yerel kişi döndürmez");

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
