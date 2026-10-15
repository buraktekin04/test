using KLMN.Application.MilitarySchools.Commands.CreateMilitarySchoolDef;
using KLMN.Application.MilitarySchools.Commands.UpdateMilitarySchoolDef;
using KLMN.Application.MilitarySchools.Commands.ChangeStatusMilitarySchoolDef;
using KLMN.Application.MilitarySchools.Commands.DeleteMilitarySchoolDef;
using KLMN.Application.MilitarySchools.Queries.GetMilitarySchoolDefs;
using KLMN.Application.MilitarySchools.Responses;
using KLMN.Domain.Entities.MilitaryUnit;

static void Check(bool passed, string scenario)
{
    if (!passed) throw new InvalidOperationException("FAIL " + scenario);
    Console.WriteLine("PASS " + scenario);
}
var start = new DateTime(2026, 10, 15, 0, 0, 0, DateTimeKind.Utc);
var end = start.AddDays(1);
var valid = new CreateMilitarySchoolDefCommand("MS-01", "Askerî Okul", Guid.NewGuid(), Guid.NewGuid(), start, end);
var cv = new CreateMilitarySchoolDefCommandValidator();
Check(cv.Validate(valid).IsValid, "Geçerli okul tanımı");
Check(!cv.Validate(valid with { Code = "" }).IsValid, "Okul kodu zorunlu");
Check(!cv.Validate(valid with { Name = "" }).IsValid, "Okul adı zorunlu");
Check(!cv.Validate(valid with { Code = "INVALID CODE" }).IsValid, "Kod biçimi");
Check(cv.Validate(valid with { CityId = null, ForcesCommandDefId = null, ValidFrom = null, ValidTo = null }).IsValid,
    "İki FK ve iki tarih isteğe bağlı");
Check(!cv.Validate(valid with { CityId = Guid.Empty }).IsValid, "Boş şehir GUID engeli");
Check(!cv.Validate(valid with { ForcesCommandDefId = Guid.Empty }).IsValid, "Boş kuvvet GUID engeli");
Check(!cv.Validate(valid with { ValidTo = start }).IsValid, "Bitiş başlangıçtan sonra olmalı");
Check(!cv.Validate(valid with { ValidFrom = DateTime.SpecifyKind(start, DateTimeKind.Local) }).IsValid,
    "UTC dışındaki başlangıç reddi");
var update = new UpdateMilitarySchoolDefCommand(Guid.NewGuid(), 2, "MS-01", "Askerî Okul", null, null, null, null);
var uv = new UpdateMilitarySchoolDefCommandValidator();
Check(uv.Validate(update).IsValid, "Güncelleme alanları ve xmin");
Check(!uv.Validate(update with { Version = 0 }).IsValid, "Sıfır xmin reddedildi");
Check(!uv.Validate(update with { Id = Guid.Empty }).IsValid, "Boş Id reddedildi");
Check(!new DeleteMilitarySchoolDefCommandValidator().Validate(
    new DeleteMilitarySchoolDefCommand(Guid.NewGuid(), 0)).IsValid, "Silme xmin zorunlu");
Check(!new ChangeStatusMilitarySchoolDefCommandValidator().Validate(
    new ChangeStatusMilitarySchoolDefCommand(Guid.Empty, 2, true)).IsValid, "Durum Id zorunlu");
Check(!new GetMilitarySchoolDefsQueryValidator().Validate(
    new GetMilitarySchoolDefsQuery(PageSize: 101)).IsValid, "100 kayıt sayfa üst sınırı");
var entity = new MilitarySchoolDef { Code = "MS-01", Name = "Askerî Okul", ValidFrom = start };
var dto = MilitarySchoolDefResponse.Projection.Compile()(entity);
Check(dto.Id == entity.Id && dto.ValidFrom == start, "Okul DTO projection");
Console.WriteLine("Askerî Okul Tanımları regresyon test kaynakları hazır.");
