using KLMN.Application.AdmissionTypes.Commands.CreateHcAdmissionTypeDef;
using KLMN.Application.AdmissionTypes.Commands.UpdateHcAdmissionTypeDef;
using KLMN.Application.AdmissionTypes.Commands.ChangeStatusHcAdmissionTypeDef;
using KLMN.Application.AdmissionTypes.Commands.DeleteHcAdmissionTypeDef;
using KLMN.Application.AdmissionTypes.Queries.GetHcAdmissionTypeDefs;
using KLMN.Application.AdmissionTypes.Responses;
using KLMN.Domain.Entities.Admissions;

static void Check(bool passed, string scenario)
{
    if (!passed) throw new InvalidOperationException("FAIL " + scenario);
    Console.WriteLine("PASS " + scenario);
}

var begin = new DateTime(2026, 10, 16, 0, 0, 0, DateTimeKind.Utc);
var until = begin.AddDays(3);
var sample = new CreateHcAdmissionTypeDefCommand(
    101, "Genel Başvuru",
    true, false, false, true, false, false, false, false, false, false, false, false, false, true,
    begin, until);
var cv = new CreateHcAdmissionTypeDefCommandValidator();
Check(cv.Validate(sample).IsValid, "Geçerli kod/ad/14 zorunluluk ayarı");
Check(!cv.Validate(sample with { Code = 0 }).IsValid, "Sıfır kod reddedilir");
Check(!cv.Validate(sample with { Code = -1 }).IsValid, "Negatif kod reddedilir");
Check(!cv.Validate(sample with { Name = "" }).IsValid, "Boş ad reddedilir");
Check(cv.Validate(sample with { ValidFrom = null, ValidTo = null }).IsValid, "Tarihler isteğe bağlı");
Check(!cv.Validate(sample with { ValidTo = begin }).IsValid, "Tarih sırası doğrulanır");
Check(!cv.Validate(sample with { ValidFrom = DateTime.SpecifyKind(begin, DateTimeKind.Local) }).IsValid,
    "Yerel tarih reddedilir");
var update = new UpdateHcAdmissionTypeDefCommand(
    Guid.NewGuid(), 3, 101, "Genel Başvuru",
    true, false, false, true, false, false, false, false, false, false, false, false, false, true,
    begin, until);
var uv = new UpdateHcAdmissionTypeDefCommandValidator();
Check(uv.Validate(update).IsValid, "Güncelleme sürümü ve 14 bayrak");
Check(!uv.Validate(update with { Version = 0 }).IsValid, "Güncellemede xmin zorunlu");
Check(!uv.Validate(update with { Id = Guid.Empty }).IsValid, "Güncellemede kimlik zorunlu");
Check(!new DeleteHcAdmissionTypeDefCommandValidator().Validate(
    new DeleteHcAdmissionTypeDefCommand(Guid.NewGuid(), 0)).IsValid, "Silmede xmin");
Check(!new ChangeStatusHcAdmissionTypeDefCommandValidator().Validate(
    new ChangeStatusHcAdmissionTypeDefCommand(Guid.Empty, 2, true)).IsValid, "Durum değişiminde Id");
Check(!new GetHcAdmissionTypeDefsQueryValidator().Validate(
    new GetHcAdmissionTypeDefsQuery(PageSize: 101)).IsValid, "100 kayıt sayfa üst limiti");
var entity = new HcAdmissionTypeDef { Code = 101, Name = "Genel Başvuru",
    ForcesCommandRequired = true, MilitaryUnitRequired = true, PertemApprovalRequired = true };
var projected = HcAdmissionTypeDefResponse.Projection.Compile()(entity);
Check(projected.Id == entity.Id && projected.Code == 101 &&
      projected.ForcesCommandRequired && projected.MilitaryUnitRequired &&
      projected.PertemApprovalRequired && !projected.RankRequired, "14 bayrak DTO projeksiyonu");
Console.WriteLine("Başvuru Grubu Tanımları kaynak regresyon senaryoları hazır.");
