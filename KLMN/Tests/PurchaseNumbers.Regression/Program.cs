using KLMN.Application.PurchaseNumbers.Commands.CreatePurchaseNumberDef;
using KLMN.Application.PurchaseNumbers.Commands.UpdatePurchaseNumberDef;
using KLMN.Application.PurchaseNumbers.Commands.DeletePurchaseNumberDef;
using KLMN.Application.PurchaseNumbers.Commands.ChangePurchaseNumberDefStatus;
using KLMN.Application.PurchaseNumbers.Queries.GetPurchaseNumberDefs;
using KLMN.Application.PurchaseNumbers.Responses;
using KLMN.Domain.Entities.PurchaseNumbers;

static void Check(bool condition, string title)
{
    if (!condition) throw new Exception("FAIL: " + title);
    Console.WriteLine("PASS: " + title);
}

var start = new DateTime(2026, 1, 1, 8, 0, 0, DateTimeKind.Utc);
var end = start.AddMonths(1);
var correct = new CreatePurchaseNumberDefCommand(
    "ALIM-001", "2026 Alımı", start, end, null, "Tedarik açıklaması");
var validator = new CreatePurchaseNumberDefCommandValidator();
Check(validator.Validate(correct).IsValid, "Geçerli UTC tarihli alım kaydı");
Check(!validator.Validate(correct with { Code = "" }).IsValid, "Kod boş olamaz");
Check(!validator.Validate(correct with { Code = "YANLIŞ KOD" }).IsValid, "Kod karakter kısıtı");
Check(!validator.Validate(correct with { Name = "" }).IsValid, "Ad zorunlu");
Check(!validator.Validate(correct with { SupplyDescriptionName = "" }).IsValid, "Tedarik açıklaması zorunlu");
Check(!validator.Validate(correct with { ValidTo = start }).IsValid, "Bitiş başlangıçtan sonra olmalı");
Check(validator.Validate(correct with { ValidFrom = null, ValidTo = null }).IsValid,
    "Tarihlerin boş olması sınırsız geçerliliği ifade eder");
Check(validator.Validate(correct with { ValidFrom = null, ValidTo = end }).IsValid,
    "Yalnızca bitiş tarihi belirtilebilir");
Check(validator.Validate(correct with { ValidFrom = start, ValidTo = null }).IsValid,
    "Yalnızca başlangıç tarihi belirtilebilir");
Check(!validator.Validate(correct with { ValidFrom = DateTime.SpecifyKind(start, DateTimeKind.Local) }).IsValid,
    "Başlangıç UTC olmalı");
Check(!validator.Validate(correct with { HealthReportEndDate =
    DateTime.SpecifyKind(end, DateTimeKind.Unspecified) }).IsValid, "Sağlık raporu UTC olmalı");
Check(validator.Validate(correct with { HealthReportEndDate = end }).IsValid, "Opsiyonel sağlık raporu tarihi");
var update = new UpdatePurchaseNumberDefCommand(Guid.NewGuid(), 4,
    correct.Code, correct.Name, start, end, null, correct.SupplyDescriptionName);
var updateValidator = new UpdatePurchaseNumberDefCommandValidator();
Check(updateValidator.Validate(update).IsValid, "Geçerli Update ve xmin");
Check(!updateValidator.Validate(update with { Version = 0u }).IsValid, "Sıfır xmin reddedilir");
Check(!updateValidator.Validate(update with { Id = Guid.Empty }).IsValid, "Boş ID reddedilir");
Check(!new DeletePurchaseNumberDefCommandValidator().Validate(
    new DeletePurchaseNumberDefCommand(Guid.NewGuid(), 0)).IsValid, "Silmeye xmin gerekir");
Check(!new ChangePurchaseNumberDefStatusCommandValidator().Validate(
    new ChangePurchaseNumberDefStatusCommand(Guid.Empty, 1, false)).IsValid, "Durum değişiminde ID gerekir");
Check(!new GetPurchaseNumberDefsQueryValidator().Validate(
    new GetPurchaseNumberDefsQuery(PageSize: 101)).IsValid, "Sayfa limiti 100");
var purchase = new PurchaseNumberDef
{
    Code = correct.Code, Name = correct.Name,
    ValidFrom = correct.ValidFrom, ValidTo = correct.ValidTo,
    SupplyDescriptionName = correct.SupplyDescriptionName,
    HealthReportEndDate = null
};
var projection = PurchaseNumberDefResponse.Projection.Compile()(purchase);
Check(projection.Code == correct.Code &&
    projection.SupplyDescriptionName == correct.SupplyDescriptionName &&
    projection.Id == purchase.Id, "KLMN Response projection");
Console.WriteLine("Alım numarası doğrulama regresyonları tamamlandı.");
