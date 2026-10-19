using KLMN.Application.ConnectedDecisions.Commands.CreateHcConnectedDecisionDef;
using KLMN.Application.ConnectedDecisions.Commands.UpdateHcConnectedDecisionDef;
using KLMN.Application.ConnectedDecisions.Commands.DeleteHcConnectedDecisionDef;
using KLMN.Application.ConnectedDecisions.Commands.ChangeStatusHcConnectedDecisionDef;
using KLMN.Application.ConnectedDecisions.Queries.GetHcConnectedDecisionDefs;
using KLMN.Application.ConnectedDecisions.Responses;
using KLMN.Domain.Entities.Admissions;

static void Check(bool valid, string scenario)
{
    if (!valid) throw new InvalidOperationException("FAIL: " + scenario);
    Console.WriteLine("PASS: " + scenario);
}

var start = new DateTime(2026, 10, 19, 0, 0, 0, DateTimeKind.Utc);
var end = start.AddDays(30);
var create = new CreateHcConnectedDecisionDefCommand("BK-01", "Bağlı Kurul Kararı", start, end);
var validator = new CreateHcConnectedDecisionDefCommandValidator();
Check(validator.Validate(create).IsValid, "Geçerli karar kodu/adı/dönemi");
Check(!validator.Validate(create with { Code = "" }).IsValid, "Kod zorunlu");
Check(!validator.Validate(create with { Code = "HATALI KOD" }).IsValid, "Kod biçimi");
Check(!validator.Validate(create with { Name = "" }).IsValid, "Ad zorunlu");
Check(!validator.Validate(create with { Code = new string('X', 101) }).IsValid, "Kod uzunluğu");
Check(validator.Validate(create with { ValidFrom = null, ValidTo = null }).IsValid,
    "Geçerlilik tarihleri isteğe bağlı");
Check(!validator.Validate(create with { ValidTo = start }).IsValid,
    "Geçerlilik sınırları");
Check(!validator.Validate(create with { ValidFrom = DateTime.SpecifyKind(start, DateTimeKind.Local) }).IsValid,
    "UTC tarih doğrulaması");

var update = new UpdateHcConnectedDecisionDefCommand(Guid.NewGuid(), 2, "BK-01", "Bağlı Karar", null, null);
var uv = new UpdateHcConnectedDecisionDefCommandValidator();
Check(uv.Validate(update).IsValid, "Güncelleme sürümü ve alanları");
Check(!uv.Validate(update with { Version = 0 }).IsValid, "Sıfır xmin sürüm reddi");
Check(!uv.Validate(update with { Id = Guid.Empty }).IsValid, "Boş kimlik reddi");

var dv = new DeleteHcConnectedDecisionDefCommandValidator();
Check(!dv.Validate(new DeleteHcConnectedDecisionDefCommand(Guid.NewGuid(), 0)).IsValid,
    "Soft-delete concurrency sürümü");

var sv = new ChangeStatusHcConnectedDecisionDefCommandValidator();
Check(!sv.Validate(new ChangeStatusHcConnectedDecisionDefCommand(Guid.Empty, 2, true)).IsValid,
    "Aktif/pasif kaydı için kimlik");

var lv = new GetHcConnectedDecisionDefsQueryValidator();
Check(!lv.Validate(new GetHcConnectedDecisionDefsQuery(PageSize: 101)).IsValid,
    "Sayfa üst sınırı 100");

var entity = new HcConnectedDecisionDef { Code = "BK-01", Name = "Bağlı Kurul Kararı", ValidFrom = start };
var item = HcConnectedDecisionDefResponse.Projection.Compile()(entity);
Check(item.Id == entity.Id && item.Code == entity.Code &&
      item.Name == entity.Name && item.ValidFrom == start,
    "DTO projeksiyon temel alanları");

Console.WriteLine("Bağlı Kurul Karar Tanımları regresyon senaryoları tamamlandı.");
