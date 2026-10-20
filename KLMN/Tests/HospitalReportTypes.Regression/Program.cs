using KLMN.Application.HospitalReportTypes.Commands.CreateHospitalReportType;
using KLMN.Application.HospitalReportTypes.Commands.UpdateHospitalReportType;
using KLMN.Application.HospitalReportTypes.Commands.ChangeStatusHospitalReportType;
using KLMN.Application.HospitalReportTypes.Queries.GetHospitalReportTypes;
using KLMN.Application.HospitalReportTypes.Queries.GetHospitalReportTypeGroups;
using KLMN.Application.HospitalReportTypes.Responses;
using KLMN.Domain.Entities.Admissions;

static void AssertCheck(bool condition, string title)
{
    if (!condition) throw new Exception("FAIL: " + title);
    Console.WriteLine("PASS: " + title);
}
var hospitalId = Guid.NewGuid();
var reportId = Guid.NewGuid();
var create = new CreateHospitalReportTypeCommand(hospitalId, reportId);
var cv = new CreateHospitalReportTypeCommandValidator();
AssertCheck(cv.Validate(create).IsValid, "İki FK ile kayıt");
AssertCheck(!cv.Validate(create with { HospitalDefId = Guid.Empty }).IsValid, "Sağlık tesisi zorunlu");
AssertCheck(!cv.Validate(create with { ReportTypeDefId = Guid.Empty }).IsValid, "Rapor türü zorunlu");
var update = new UpdateHospitalReportTypeCommand(Guid.NewGuid(), 5, hospitalId, reportId);
AssertCheck(new UpdateHospitalReportTypeCommandValidator().Validate(update).IsValid, "Güncelleme FK ve xmin");
AssertCheck(!new UpdateHospitalReportTypeCommandValidator().Validate(update with { Version = 0 }).IsValid, "Eski/sıfır versiyon");
AssertCheck(!new ChangeStatusHospitalReportTypeCommandValidator().Validate(
    new ChangeStatusHospitalReportTypeCommand(Guid.Empty, 5, true)).IsValid, "Durum değişiminde kimlik");
AssertCheck(!new GetHospitalReportTypesQueryValidator().Validate(
    new GetHospitalReportTypesQuery(PageSize:101)).IsValid, "Sorgu sayfa sınırı");
var relation = new HospitalReportType {
    HospitalDefId = hospitalId, ReportTypeDefId = reportId,
    Hospital = new HospitalDef { Id = hospitalId, Code = "ST-01", Name = "Tesis" },
    ReportType = new ReportTypeDef { Id = reportId, Code = "RT-01", Name = "Rapor" }
};
var dto = HospitalReportTypeResponse.Projection.Compile()(relation);
AssertCheck(dto.HospitalDefId == hospitalId && dto.ReportTypeDefId == reportId
    && dto.HospitalName == "Tesis" && dto.ReportTypeName == "Rapor", "Eşleştirme projection");
AssertCheck(!new GetHospitalReportTypeGroupsQueryValidator().Validate(
    new GetHospitalReportTypeGroupsQuery(PageSize:101)).IsValid,
    "Gruplu listede aynı sayfa boyutu sınırı");
var group = new HospitalReportTypeGroupResponse(hospitalId, "ST-01", "Tesis", [dto]);
AssertCheck(group.HospitalDefId == hospitalId && group.ReportTypes.Count == 1 &&
    group.ReportTypes[0].ReportTypeName == "Rapor",
    "Tesis başına bir satır / çoklu rapor response sözleşmesi");
Console.WriteLine("Sağlık tesisi–rapor türü regresyon kaynakları hazır.");
