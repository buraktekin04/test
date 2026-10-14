using KLMN.Application.Towns.Commands.CreateTownDef;
using KLMN.Application.Towns.Commands.UpdateTownDef;
using KLMN.Application.Towns.Commands.DeleteTownDef;
using KLMN.Application.Towns.Commands.ChangeStatusTownDef;
using KLMN.Application.Towns.Queries.GetTownDefs;
using KLMN.Application.Towns.Responses;
using KLMN.Domain.Entities.Geography;

// İlçe doğrulama ve optimistic concurrency alanlarının regresyon kontrolleri.
static void Check(bool condition, string name)
{
    if (!condition) throw new InvalidOperationException("FAIL " + name);
    Console.WriteLine("PASS " + name);
}
var create = new CreateTownDefCommand("ILCE_01", "Çankaya", null, Guid.NewGuid(), 12);
var cv = new CreateTownDefCommandValidator();
Check(cv.Validate(create).IsValid, "Geçerli ilçe oluşturma");
Check(!cv.Validate(create with { Code = "" }).IsValid, "Kod gerekli");
Check(!cv.Validate(create with { Code = "HATALI KOD" }).IsValid, "Kod biçimi");
Check(!cv.Validate(create with { Name = "" }).IsValid, "İlçe adı gerekli");
Check(cv.Validate(create with { CityId = null, MernisCode = null }).IsValid,
    "Nullable şehir ve MERNİS");
Check(!cv.Validate(create with { CityId = Guid.Empty }).IsValid, "Boş CityId reddi");
Check(!cv.Validate(create with { MernisCode = -1 }).IsValid, "Negatif MERNİS reddi");
var update = new UpdateTownDefCommand(Guid.NewGuid(), 1, "ILCE_01", "Çankaya", null, null, null);
Check(new UpdateTownDefCommandValidator().Validate(update).IsValid, "Geçerli güncelleme");
Check(!new UpdateTownDefCommandValidator().Validate(update with { Version = 0 }).IsValid,
    "xmin sıfır olamaz");
Check(!new DeleteTownDefCommandValidator().Validate(
    new DeleteTownDefCommand(Guid.NewGuid(), 0)).IsValid, "Silmeye xmin gerekir");
Check(!new ChangeStatusTownDefCommandValidator().Validate(
    new ChangeStatusTownDefCommand(Guid.Empty, 1, false)).IsValid, "Durum için Id gerekli");
Check(!new GetTownDefsQueryValidator().Validate(
    new GetTownDefsQuery(PageSize: 101)).IsValid, "Sayfa sınırı 100");
var entity = new TownDef { Code = "ILCE_01", Name = "Çankaya" };
var result = TownDefResponse.Projection.Compile()(entity);
Check(result.Id == entity.Id && result.Name == entity.Name, "Response projection");
Console.WriteLine("İlçe regresyon kaynak kontrolleri tamamlandı.");
