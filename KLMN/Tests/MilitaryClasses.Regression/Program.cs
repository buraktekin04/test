using KLMN.Application.MilitaryClasses.Commands.CreateMilitaryClassDef;
using KLMN.Application.MilitaryClasses.Commands.UpdateMilitaryClassDef;
using KLMN.Application.MilitaryClasses.Commands.DeleteMilitaryClassDef;
using KLMN.Application.MilitaryClasses.Commands.ChangeStatusMilitaryClassDef;
using KLMN.Application.MilitaryClasses.Queries.GetMilitaryClassDefs;
using KLMN.Application.MilitaryClasses.Responses;
using KLMN.Domain.Entities.MilitaryUnit;

static void Check(bool ok, string message)
{
    if (!ok) throw new InvalidOperationException("FAIL: " + message);
    Console.WriteLine("PASS: " + message);
}
var start = new DateTime(2026, 10, 15, 0, 0, 0, DateTimeKind.Utc);
var create = new CreateMilitaryClassDefCommand("PIYADE", "Piyade", "Pyd", "EXT", null, start, start.AddDays(1));
var cv = new CreateMilitaryClassDefCommandValidator();
Check(cv.Validate(create).IsValid, "Geçerli sınıf komutu");
Check(!cv.Validate(create with { Code = "" }).IsValid, "Kod gerekli");
Check(!cv.Validate(create with { Name = "" }).IsValid, "Ad gerekli");
Check(!cv.Validate(create with { ShortName = new string('x', 101) }).IsValid, "Kısa ad uzunluğu");
Check(!cv.Validate(create with { ExternalCode = new string('x', 101) }).IsValid, "Harici kod uzunluğu");
Check(cv.Validate(create with { Active = true, ValidFrom = null, ValidTo = null }).IsValid, "Active true/null tarih");
Check(cv.Validate(create with { Active = false }).IsValid, "Active false");
Check(!cv.Validate(create with { ValidTo = start }).IsValid, "Tarih aralığı");
Check(!cv.Validate(create with { ValidFrom = DateTime.SpecifyKind(start, DateTimeKind.Local) }).IsValid, "UTC gerekli");
var update = new UpdateMilitaryClassDefCommand(Guid.NewGuid(), 2, "PIYADE", "Piyade", null, null, false, null, null);
var uv = new UpdateMilitaryClassDefCommandValidator();
Check(uv.Validate(update).IsValid, "Güncelleme geçerli");
Check(!uv.Validate(update with { Version = 0 }).IsValid, "xmin gerekli");
Check(!uv.Validate(update with { Id = Guid.Empty }).IsValid, "Id gerekli");
Check(!new DeleteMilitaryClassDefCommandValidator().Validate(
    new DeleteMilitaryClassDefCommand(Guid.NewGuid(), 0)).IsValid, "Delete xmin");
Check(!new ChangeStatusMilitaryClassDefCommandValidator().Validate(
    new ChangeStatusMilitaryClassDefCommand(Guid.Empty, 1, true)).IsValid, "Status Id");
Check(!new GetMilitaryClassDefsQueryValidator().Validate(
    new GetMilitaryClassDefsQuery(PageSize: 101)).IsValid, "Sayfa üst sınır");
var entity = new MilitaryClassDef { Code = "PIYADE", Name = "Piyade", Active = null, ValidFrom = start };
var dto = MilitaryClassDefResponse.Projection.Compile()(entity);
Check(dto.Active == null && dto.Id == entity.Id, "DTO projection nullable Active");
Console.WriteLine("Askerî Sınıf regresyon test kaynakları hazır.");
