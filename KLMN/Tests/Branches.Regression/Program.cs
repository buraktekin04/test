using KLMN.Application.Branches.Commands.CreateBranchDef;
using KLMN.Application.Branches.Commands.UpdateBranchDef;
using KLMN.Application.Branches.Commands.DeleteBranchDef;
using KLMN.Application.Branches.Commands.ChangeStatusBranchDef;
using KLMN.Application.Branches.Queries.GetBranchDefs;
using KLMN.Application.Branches.Responses;
using KLMN.Domain.Entities.Admissions;

static void Check(bool valid, string scenario)
{
    if (!valid) throw new InvalidOperationException("FAIL: " + scenario);
    Console.WriteLine("PASS: " + scenario);
}

var start = new DateTime(2026, 10, 19, 0, 0, 0, DateTimeKind.Utc);
var end = start.AddDays(30);
var create = new CreateBranchDefCommand("BK-01", "Branş", start, end);
var validator = new CreateBranchDefCommandValidator();
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

var update = new UpdateBranchDefCommand(Guid.NewGuid(), 2, "BK-01", "Bağlı Karar", null, null);
var uv = new UpdateBranchDefCommandValidator();
Check(uv.Validate(update).IsValid, "Güncelleme sürümü ve alanları");
Check(!uv.Validate(update with { Version = 0 }).IsValid, "Sıfır xmin sürüm reddi");
Check(!uv.Validate(update with { Id = Guid.Empty }).IsValid, "Boş kimlik reddi");

var dv = new DeleteBranchDefCommandValidator();
Check(!dv.Validate(new DeleteBranchDefCommand(Guid.NewGuid(), 0)).IsValid,
    "Soft-delete concurrency sürümü");

var sv = new ChangeStatusBranchDefCommandValidator();
Check(!sv.Validate(new ChangeStatusBranchDefCommand(Guid.Empty, 2, true)).IsValid,
    "Aktif/pasif kaydı için kimlik");

var lv = new GetBranchDefsQueryValidator();
Check(!lv.Validate(new GetBranchDefsQuery(PageSize: 101)).IsValid,
    "Sayfa üst sınırı 100");

var entity = new BranchDef { Code = "BK-01", Name = "Branş", ValidFrom = start };
var item = BranchDefResponse.Projection.Compile()(entity);
Check(item.Id == entity.Id && item.Code == entity.Code &&
      item.Name == entity.Name && item.ValidFrom == start,
    "DTO projeksiyon temel alanları");

Console.WriteLine("Branş Tanımları regresyon senaryoları tamamlandı.");
