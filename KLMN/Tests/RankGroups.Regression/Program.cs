using KLMN.Application.RankGroups.Commands.CreateRankGroupDef;
using KLMN.Application.RankGroups.Commands.UpdateRankGroupDef;
using KLMN.Application.RankGroups.Commands.DeleteRankGroupDef;
using KLMN.Application.RankGroups.Commands.ChangeStatusRankGroupDef;
using KLMN.Application.RankGroups.Queries.GetRankGroupDefs;
using KLMN.Application.RankGroups.Responses;
using KLMN.Domain.Entities.Admissions;

static void Check(bool valid, string scenario)
{
    if (!valid) throw new InvalidOperationException("FAIL: " + scenario);
    Console.WriteLine("PASS: " + scenario);
}

var start = new DateTime(2026, 10, 20, 0, 0, 0, DateTimeKind.Utc);
var end = start.AddDays(30);
var create = new CreateRankGroupDefCommand("RT-01", "Rütbe Grubu", null, start, end);
var validator = new CreateRankGroupDefCommandValidator();
Check(validator.Validate(create).IsValid, "Geçerli kod/ad/dönem");
Check(!validator.Validate(create with { Code = "" }).IsValid, "Kod zorunlu");
Check(!validator.Validate(create with { Code = "HATALI KOD" }).IsValid, "Kod biçimi");
Check(!validator.Validate(create with { Name = "" }).IsValid, "Ad zorunlu");
Check(!validator.Validate(create with { ShortName = new string('X', 101) }).IsValid, "Kısa ad sınırı");
Check(!validator.Validate(create with { Code = new string('X', 101) }).IsValid, "Kod uzunluğu");
Check(validator.Validate(create with { ValidFrom = null, ValidTo = null }).IsValid,
    "Geçerlilik tarihleri isteğe bağlı");
Check(!validator.Validate(create with { ValidTo = start }).IsValid,
    "Geçerlilik sınırları");
Check(!validator.Validate(create with { ValidFrom = DateTime.SpecifyKind(start, DateTimeKind.Local) }).IsValid,
    "UTC tarih doğrulaması");

var update = new UpdateRankGroupDefCommand(Guid.NewGuid(), 2, "RT-01", "Rütbe Grubu", null, null, null);
var uv = new UpdateRankGroupDefCommandValidator();
Check(uv.Validate(update).IsValid, "Güncelleme sürümü ve alanları");
Check(!uv.Validate(update with { Version = 0 }).IsValid, "Sıfır xmin sürüm reddi");
Check(!uv.Validate(update with { Id = Guid.Empty }).IsValid, "Boş kimlik reddi");

var dv = new DeleteRankGroupDefCommandValidator();
Check(!dv.Validate(new DeleteRankGroupDefCommand(Guid.NewGuid(), 0)).IsValid,
    "Soft-delete concurrency sürümü");

var sv = new ChangeStatusRankGroupDefCommandValidator();
Check(!sv.Validate(new ChangeStatusRankGroupDefCommand(Guid.Empty, 2, true)).IsValid,
    "Aktif/pasif kaydı için kimlik");

var lv = new GetRankGroupDefsQueryValidator();
Check(!lv.Validate(new GetRankGroupDefsQuery(PageSize: 101)).IsValid,
    "Sayfa üst sınırı 100");

var entity = new RankGroupDef { Code = "RT-01", Name = "Rütbe Grubu", ShortName = "RG", ValidFrom = start };
var item = RankGroupDefResponse.Projection.Compile()(entity);
Check(item.Id == entity.Id && item.Code == entity.Code &&
      item.Name == entity.Name && item.ValidFrom == start,
    "DTO projeksiyon temel alanları");

Console.WriteLine("Rütbe Grubu Tanımları regresyon senaryoları tamamlandı.");
