using KLMN.Application.MilitaryOffices.Commands.CreateMilitaryOfficeDef;
using KLMN.Application.MilitaryOffices.Commands.UpdateMilitaryOfficeDef;
using KLMN.Application.MilitaryOffices.Commands.ChangeStatusMilitaryOfficeDef;
using KLMN.Application.MilitaryOffices.Commands.DeleteMilitaryOfficeDef;
using KLMN.Application.MilitaryOffices.Queries.GetMilitaryOfficeDefs;
using KLMN.Application.MilitaryOffices.Responses;
using KLMN.Domain.Entities.MilitaryUnit;

static void Check(bool ok, string label)
{
    if (!ok) throw new InvalidOperationException("FAIL: " + label);
    Console.WriteLine("PASS: " + label);
}

var start = new DateTime(2026, 10, 16, 0, 0, 0, DateTimeKind.Utc);
var end = start.AddDays(5);
var create = new CreateMilitaryOfficeDefCommand("MO-01", "Merkez Askerlik Şubesi", "EXT-15",
    Guid.NewGuid(), start, end);
var cv = new CreateMilitaryOfficeDefCommandValidator();
Check(cv.Validate(create).IsValid, "Şube oluşturma geçerli");
Check(!cv.Validate(create with { Code = "" }).IsValid, "Kod gerekli");
Check(!cv.Validate(create with { Name = "" }).IsValid, "Ad gerekli");
Check(!cv.Validate(create with { Code = "INVALID CODE" }).IsValid, "Kod biçimi");
Check(!cv.Validate(create with { ExternalCode = new string('x', 101) }).IsValid,
    "Harici kod uzunluğu");
Check(cv.Validate(create with { CityId = null, ExternalCode = null, ValidFrom = null, ValidTo = null }).IsValid,
    "Şehir/harici kod ve iki tarih isteğe bağlı");
Check(!cv.Validate(create with { CityId = Guid.Empty }).IsValid, "Boş şehir GUID'i reddi");
Check(!cv.Validate(create with { ValidTo = start }).IsValid, "Tarih sınırı");
Check(!cv.Validate(create with { ValidFrom = DateTime.SpecifyKind(start, DateTimeKind.Local) }).IsValid,
    "UTC başlangıç gereği");
var update = new UpdateMilitaryOfficeDefCommand(Guid.NewGuid(), 2, "MO-01", "Şube", null,
    null, null, null);
var uv = new UpdateMilitaryOfficeDefCommandValidator();
Check(uv.Validate(update).IsValid, "Güncelleme geçerli");
Check(!uv.Validate(update with { Version = 0 }).IsValid, "Güncelleme xmin zorunlu");
Check(!uv.Validate(update with { Id = Guid.Empty }).IsValid, "Güncelleme kimliği zorunlu");
Check(!new DeleteMilitaryOfficeDefCommandValidator().Validate(
    new DeleteMilitaryOfficeDefCommand(Guid.NewGuid(), 0)).IsValid, "Silmede xmin");
Check(!new ChangeStatusMilitaryOfficeDefCommandValidator().Validate(
    new ChangeStatusMilitaryOfficeDefCommand(Guid.Empty, 2, true)).IsValid, "Durumda ID");
Check(!new GetMilitaryOfficeDefsQueryValidator().Validate(
    new GetMilitaryOfficeDefsQuery(PageSize: 101)).IsValid, "Sayfa üst limiti");
var office = new MilitaryOfficeDef { Code = "MO-01", Name = "Şube", ExternalCode = "EX", ValidFrom = start };
var response = MilitaryOfficeDefResponse.Projection.Compile()(office);
Check(response.Id == office.Id && response.ExternalCode == "EX" && response.ValidFrom == start,
    "DTO projeksiyon alanları");
Console.WriteLine("Askerlik Şubesi Tanımları kaynak tabanlı regresyon senaryoları tamamlandı.");
