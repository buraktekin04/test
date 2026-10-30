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
Check(!new GetDispatchOptionsQueryValidator().Validate(new GetDispatchOptionsQuery("bad-type")).IsValid,
    "Tanım türü allowlist dışında sorgu reddi");
Check(typeof(BaseEntity).IsAssignableFrom(typeof(Person)),"Kişi audit ve xmin kalıtımı");
Check(typeof(BaseEntity).IsAssignableFrom(typeof(Dispatch)),"Sevk audit ve xmin kalıtımı");
Check(typeof(BaseEntity).IsAssignableFrom(typeof(DispatchFile)),"Sevk dosyası audit ve xmin kalıtımı");
Check(typeof(Dispatch).GetProperty(nameof(Dispatch.HcAdmissionTypeDefId))?.PropertyType==typeof(Guid),
    "Sevk başvuru grubu FK");
Check(typeof(Dispatch).GetProperty(nameof(Dispatch.PersonId))?.PropertyType==typeof(Guid),
    "Sevk kişi FK");
Console.WriteLine("Not: Yerel build, PostgreSQL migration, kimlik/başvuru kuralı, izin ve dosya entegrasyon senaryoları ayrıca test edilmelidir.");
