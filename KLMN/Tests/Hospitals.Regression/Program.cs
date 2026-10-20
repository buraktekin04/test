using KLMN.Application.Hospitals.Commands.CreateHospitalDef;
using KLMN.Application.Hospitals.Commands.UpdateHospitalDef;
using KLMN.Application.Hospitals.Commands.ChangeStatusHospitalDef;
using KLMN.Application.Hospitals.Queries.GetHospitalDefs;
using KLMN.Application.Hospitals.Responses;
using KLMN.Domain.Entities.Admissions;

static void AssertCheck(bool condition, string title)
{
    if (!condition) throw new Exception("FAIL: " + title);
    Console.WriteLine("PASS: " + title);
}
var from = new DateTime(2026, 10, 20, 0, 0, 0, DateTimeKind.Utc);
var created = new CreateHospitalDefCommand("ST-01", "Test Sağlık Tesisi", null, null, from, from.AddDays(1));
var cv = new CreateHospitalDefCommandValidator();
AssertCheck(cv.Validate(created).IsValid, "Nullable şehir/hakem ve geçerlilik dönemi");
AssertCheck(!cv.Validate(created with { Code = "" }).IsValid, "Kod zorunlu");
AssertCheck(!cv.Validate(created with { Name = "" }).IsValid, "Ad zorunlu");
AssertCheck(!cv.Validate(created with { CityId = Guid.Empty }).IsValid, "Şehir GUID kontrolü");
AssertCheck(!cv.Validate(created with { RefereeHospitalId = Guid.Empty }).IsValid, "Hakem GUID kontrolü");
AssertCheck(!cv.Validate(created with { ValidTo = from }).IsValid, "Geçerlilik tarih sırası");
var update = new UpdateHospitalDefCommand(Guid.NewGuid(), 4, "ST-01", "Test Sağlık Tesisi", null, null, from, null);
AssertCheck(new UpdateHospitalDefCommandValidator().Validate(update).IsValid, "Güncellemede xmin ve nullable FK");
AssertCheck(!new UpdateHospitalDefCommandValidator().Validate(update with { Version = 0 }).IsValid, "xmin zorunlu");
AssertCheck(!new GetHospitalDefsQueryValidator().Validate(new GetHospitalDefsQuery(PageSize:101)).IsValid, "100 sayfa limiti");
var hospital = new HospitalDef { Code = "ST-01", Name = "Test Sağlık Tesisi", CityId = Guid.NewGuid() };
var dto = HospitalDefResponse.Projection.Compile()(hospital);
AssertCheck(dto.CityId == hospital.CityId && dto.Name == hospital.Name, "Sağlık tesisi projection");
Console.WriteLine("Sağlık tesisi kaynak regresyon senaryoları hazır.");
