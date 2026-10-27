using KLMN.Application.Cities.Commands.CreateCityDef;
using KLMN.Application.Cities.Commands.UpdateCityDef;
using KLMN.Application.Towns.Commands.CreateTownDef;
using KLMN.Application.Towns.Commands.UpdateTownDef;
using KLMN.Application.MilitaryOffices.Commands.CreateMilitaryOfficeDef;
using KLMN.Application.MilitaryOffices.Commands.UpdateMilitaryOfficeDef;

static void Check(bool test,string title)
{
    if(!test)throw new Exception(title);
    Console.WriteLine("PASS "+title);
}
var parent=Guid.NewGuid();
var id=Guid.NewGuid();
var city=new CreateCityDefCommand("C1","Şehir",null,null,null,null,null);
var town=new CreateTownDefCommand("T1","İlçe",null,null,null);
var office=new CreateMilitaryOfficeDefCommand("O1","Şube",null,null,null,null);
var cv=new CreateCityDefCommandValidator();
var tv=new CreateTownDefCommandValidator();
var ov=new CreateMilitaryOfficeDefCommandValidator();
Check(!cv.Validate(city).IsValid,"Şehir için ülke zorunlu");
Check(cv.Validate(city with{CountryId=parent}).IsValid,"Şehir ülkeyle oluşturulabilir");
Check(!tv.Validate(town).IsValid,"İlçe için şehir zorunlu");
Check(tv.Validate(town with{CityId=parent}).IsValid,"İlçe şehirle oluşturulabilir");
Check(!ov.Validate(office).IsValid,"Askerlik şubesi için şehir zorunlu");
Check(ov.Validate(office with{CityId=parent}).IsValid,"Askerlik şubesi şehirle oluşturulabilir");
var cu=new UpdateCityDefCommand(id,1,"C1","Şehir",null,null,null,null,null);
var tu=new UpdateTownDefCommand(id,1,"T1","İlçe",null,null,null);
var ou=new UpdateMilitaryOfficeDefCommand(id,1,"O1","Şube",null,null,null,null);
Check(!new UpdateCityDefCommandValidator().Validate(cu).IsValid,"Şehir güncellemede ülke zorunlu");
Check(!new UpdateTownDefCommandValidator().Validate(tu).IsValid,"İlçe güncellemede şehir zorunlu");
Check(!new UpdateMilitaryOfficeDefCommandValidator().Validate(ou).IsValid,"Şube güncellemede şehir zorunlu");
Check(new UpdateCityDefCommandValidator().Validate(cu with{CountryId=parent}).IsValid,"Şehir güncelleme geçerli ülke");
Check(new UpdateTownDefCommandValidator().Validate(tu with{CityId=parent}).IsValid,"İlçe güncelleme geçerli şehir");
Check(new UpdateMilitaryOfficeDefCommandValidator().Validate(ou with{CityId=parent}).IsValid,"Şube güncelleme geçerli şehir");
Console.WriteLine("Veritabanı FK ve soft-delete indeks testleri entegrasyon ortamında ayrıca çalıştırılmalıdır.");
