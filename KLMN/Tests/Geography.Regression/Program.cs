using KLMN.Application.Countries.Commands.CreateCountryDef;
using KLMN.Application.Countries.Commands.UpdateCountryDef;
using KLMN.Application.Countries.Commands.ChangeStatusCountryDef;
using KLMN.Application.Countries.Commands.DeleteCountryDef;
using KLMN.Application.Countries.Queries.GetCountryDefs;
using KLMN.Application.Cities.Commands.CreateCityDef;
using KLMN.Application.Cities.Commands.UpdateCityDef;
using KLMN.Application.Cities.Commands.ChangeStatusCityDef;
using KLMN.Application.Cities.Commands.DeleteCityDef;
using KLMN.Application.Cities.Queries.GetCityDefs;
using KLMN.Application.Countries.Responses;
using KLMN.Application.Cities.Responses;
using KLMN.Domain.Entities.Geography;

static void Check(bool result, string name)
{
    if (!result) throw new Exception("FAIL " + name);
    Console.WriteLine("PASS " + name);
}
// Mevcut ExternalCode alanı (ör. eski sistem TUR) korunur; ShortCode ayrı alan olmalıdır.
var country = new CreateCountryDefCommand("TR", "Türkiye", "TUR", true, "TR");
var countryValidator = new CreateCountryDefCommandValidator();
Check(countryValidator.Validate(country).IsValid, "Ülke formu geçerli");
Check(countryValidator.Validate(country with { ShortCode = null }).IsValid, "Eski ülke için nullable kısa kod");
Check(countryValidator.Validate(country with { ShortCode = "USA" }).IsValid, "Üç harfli kısa kod");
Check(!countryValidator.Validate(country with { ShortCode = "X" }).IsValid, "Tek harf kısa kod reddedilir");
Check(!countryValidator.Validate(country with { ShortCode = "T1" }).IsValid, "Rakam içeren kısa kod reddedilir");
Check(!countryValidator.Validate(country with { ShortCode = "TOOLONGCOUNTRY" }).IsValid, "Uzun kısa kod reddedilir");
Check(!countryValidator.Validate(country with { Code = "" }).IsValid, "Ülke kodu zorunlu");
Check(!countryValidator.Validate(country with { Name = "" }).IsValid, "Ülke adı zorunlu");
Check(countryValidator.Validate(country with { IsTownRequired = null }).IsValid, "Nullable ilçe zorunluluğu");
Check(!new UpdateCountryDefCommandValidator().Validate(new UpdateCountryDefCommand(Guid.Empty, 0, "TR", "Türkiye", null, null, "TR")).IsValid,
    "Ülke güncellemesi Id ve xmin ister");
Check(!new DeleteCountryDefCommandValidator().Validate(new DeleteCountryDefCommand(Guid.NewGuid(), 0)).IsValid,
    "Ülke silmesi xmin ister");
Check(!new ChangeStatusCountryDefCommandValidator().Validate(new ChangeStatusCountryDefCommand(Guid.Empty, 2, false)).IsValid,
    "Ülke durumunda Guid kontrolü");
var start = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
var end = start.AddDays(1);
var city = new CreateCityDefCommand("06", "Ankara", "ANK", Guid.NewGuid(), 6, start, end);
var cityValidator = new CreateCityDefCommandValidator();
Check(cityValidator.Validate(city).IsValid, "Şehir formu geçerli");
Check(cityValidator.Validate(city with { CountryId = null, ValidFrom = null, ValidTo = null }).IsValid,
    "Ülke ve geçerlilik tarihleri nullable");
Check(!cityValidator.Validate(city with { CountryId = Guid.Empty }).IsValid, "Boş Guid ülke olamaz");
Check(!cityValidator.Validate(city with { MernisCode = -1 }).IsValid, "Negatif MERNİS olamaz");
Check(!cityValidator.Validate(city with { ValidTo = start }).IsValid, "Geçerli bitiş daha sonra olmalı");
Check(!cityValidator.Validate(city with { ValidFrom = DateTime.SpecifyKind(start, DateTimeKind.Local) }).IsValid,
    "Şehir başlangıcı UTC olmalı");
Check(!new UpdateCityDefCommandValidator().Validate(
    new UpdateCityDefCommand(Guid.Empty, 0, city.Code, city.Name, null, null, null, null, null)).IsValid,
    "Şehir güncellemesi Id ve xmin ister");
Check(!new DeleteCityDefCommandValidator().Validate(new DeleteCityDefCommand(Guid.NewGuid(), 0)).IsValid,
    "Şehir silmesi xmin ister");
Check(!new ChangeStatusCityDefCommandValidator().Validate(new ChangeStatusCityDefCommand(Guid.Empty, 1, true)).IsValid,
    "Şehir durumunda Guid kontrolü");
Check(!new GetCountryDefsQueryValidator().Validate(new GetCountryDefsQuery(PageSize:101)).IsValid,
    "Ülke sayfa üst sınırı");
Check(!new GetCityDefsQueryValidator().Validate(new GetCityDefsQuery(PageNumber:0)).IsValid,
    "Şehir sayfa alt sınırı");
var countryEntity = new CountryDef { Code="TR", Name="Türkiye", ExternalCode="LEGACY_EXTERNAL", ShortCode="TR", IsTownRequired=null };
var countryDto = CountryDefResponse.Projection.Compile()(countryEntity);
Check(countryDto.Id == countryEntity.Id && countryDto.CityCount == 0, "Ülke response projection");
Check(countryDto.ShortCode == "TR" && countryDto.ExternalCode == "LEGACY_EXTERNAL",
    "Ülke kısa kodu ve harici kod DTO'da birbirinden bağımsız");
Check(countryValidator.Validate(country with { ShortCode = "de", ExternalCode = "CUSTOM-EXT" }).IsValid,
    "Kısa kod ve harici kod farklı amaçlarla girilebilir");
var cityEntity = new CityDef { Code="06", Name="Ankara", CountryId=countryEntity.Id, ValidFrom=start };
var cityDto = CityDefResponse.Projection.Compile()(cityEntity);
Check(cityDto.Id == cityEntity.Id && cityDto.ValidFrom == start, "Şehir response projection");
Console.WriteLine("Ülke/Şehir regresyon doğrulama kontrolleri tamamlandı.");
