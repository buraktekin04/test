using KLMN.Application.Ranks.Commands.CreateRankDef;
using KLMN.Application.Ranks.Commands.UpdateRankDef;
using KLMN.Application.Ranks.Commands.DeleteRankDef;
using KLMN.Application.Ranks.Commands.ChangeStatusRankDef;
using KLMN.Application.Ranks.Queries.GetRankDefs;
using KLMN.Application.Ranks.Responses;
using KLMN.Domain.Entities.Admissions;

static void Check(bool valid, string scenario)
{
    if (!valid) throw new InvalidOperationException("FAIL: " + scenario);
    Console.WriteLine("PASS: " + scenario);
}

var start = new DateTime(2026, 10, 20, 0, 0, 0, DateTimeKind.Utc);
var end = start.AddDays(30);
var create = new CreateRankDefCommand("RT-01", "Rütbe", null, 0, null, start, end);
var validator = new CreateRankDefCommandValidator();
Check(validator.Validate(create).IsValid, "Geçerli kod/ad/dönem");
Check(!validator.Validate(create with { Code = "" }).IsValid, "Kod zorunlu");
Check(!validator.Validate(create with { Code = "HATALI KOD" }).IsValid, "Kod biçimi");
Check(!validator.Validate(create with { Name = "" }).IsValid, "Ad zorunlu");
Check(!validator.Validate(create with { ShortName = new string('X', 101) }).IsValid, "Kısa ad sınırı");
Check(!validator.Validate(create with { RankGroupId = Guid.Empty }).IsValid, "Boş grup GUID reddi");
Check(!validator.Validate(create with { Code = new string('X', 101) }).IsValid, "Kod uzunluğu");
Check(validator.Validate(create with { ValidFrom = null, ValidTo = null }).IsValid,
    "Geçerlilik tarihleri isteğe bağlı");
Check(!validator.Validate(create with { ValidTo = start }).IsValid,
    "Geçerlilik sınırları");
Check(!validator.Validate(create with { ValidFrom = DateTime.SpecifyKind(start, DateTimeKind.Local) }).IsValid,
    "UTC tarih doğrulaması");

var update = new UpdateRankDefCommand(Guid.NewGuid(), 2, "RT-01", "Rütbe", null, 0, null, null, null);
var uv = new UpdateRankDefCommandValidator();
Check(uv.Validate(update).IsValid, "Güncelleme sürümü ve alanları");
Check(!uv.Validate(update with { Version = 0 }).IsValid, "Sıfır xmin sürüm reddi");
Check(!uv.Validate(update with { Id = Guid.Empty }).IsValid, "Boş kimlik reddi");

var dv = new DeleteRankDefCommandValidator();
Check(!dv.Validate(new DeleteRankDefCommand(Guid.NewGuid(), 0)).IsValid,
    "Soft-delete concurrency sürümü");

var sv = new ChangeStatusRankDefCommandValidator();
Check(!sv.Validate(new ChangeStatusRankDefCommand(Guid.Empty, 2, true)).IsValid,
    "Aktif/pasif kaydı için kimlik");

var lv = new GetRankDefsQueryValidator();
Check(!lv.Validate(new GetRankDefsQuery(PageSize: 101)).IsValid,
    "Sayfa üst sınırı 100");

var entity = new RankDef { Code = "RT-01", Name = "Rütbe", ShortName = "RT", Type = 3, RankGroupId = Guid.NewGuid(), ValidFrom = start };
var item = RankDefResponse.Projection.Compile()(entity);
Check(item.Id == entity.Id && item.Code == entity.Code &&
      item.Name == entity.Name && item.Type == 3 && item.RankGroupId == entity.RankGroupId && item.ValidFrom == start,
    "DTO projeksiyon temel alanları");

Console.WriteLine("Rütbe Tanımları regresyon senaryoları tamamlandı.");
