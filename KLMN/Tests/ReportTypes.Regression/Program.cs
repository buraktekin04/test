using KLMN.Application.ReportTypes.Commands.CreateReportTypeDef;
using KLMN.Application.ReportTypes.Commands.UpdateReportTypeDef;
using KLMN.Application.ReportTypes.Commands.DeleteReportTypeDef;
using KLMN.Application.ReportTypes.Commands.ChangeStatusReportTypeDef;
using KLMN.Application.ReportTypes.Queries.GetReportTypeDefs;
using KLMN.Application.ReportTypes.Responses;
using KLMN.Domain.Entities.Admissions;

static void Check(bool valid, string scenario)
{
    if (!valid) throw new InvalidOperationException("FAIL: " + scenario);
    Console.WriteLine("PASS: " + scenario);
}

var start = new DateTime(2026, 10, 20, 0, 0, 0, DateTimeKind.Utc);
var end = start.AddDays(30);
var create = new CreateReportTypeDefCommand("RT-01", "Rapor Türü", start, end);
var validator = new CreateReportTypeDefCommandValidator();
Check(validator.Validate(create).IsValid, "Geçerli kod/ad/dönem");
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

var update = new UpdateReportTypeDefCommand(Guid.NewGuid(), 2, "RT-01", "Rapor Türü", null, null);
var uv = new UpdateReportTypeDefCommandValidator();
Check(uv.Validate(update).IsValid, "Güncelleme sürümü ve alanları");
Check(!uv.Validate(update with { Version = 0 }).IsValid, "Sıfır xmin sürüm reddi");
Check(!uv.Validate(update with { Id = Guid.Empty }).IsValid, "Boş kimlik reddi");

var dv = new DeleteReportTypeDefCommandValidator();
Check(!dv.Validate(new DeleteReportTypeDefCommand(Guid.NewGuid(), 0)).IsValid,
    "Soft-delete concurrency sürümü");

var sv = new ChangeStatusReportTypeDefCommandValidator();
Check(!sv.Validate(new ChangeStatusReportTypeDefCommand(Guid.Empty, 2, true)).IsValid,
    "Aktif/pasif kaydı için kimlik");

var lv = new GetReportTypeDefsQueryValidator();
Check(!lv.Validate(new GetReportTypeDefsQuery(PageSize: 101)).IsValid,
    "Sayfa üst sınırı 100");

var entity = new ReportTypeDef { Code = "RT-01", Name = "Rapor Türü", ValidFrom = start };
var item = ReportTypeDefResponse.Projection.Compile()(entity);
Check(item.Id == entity.Id && item.Code == entity.Code &&
      item.Name == entity.Name && item.ValidFrom == start,
    "DTO projeksiyon temel alanları");

Console.WriteLine("Rapor Türü Tanımları regresyon senaryoları tamamlandı.");
