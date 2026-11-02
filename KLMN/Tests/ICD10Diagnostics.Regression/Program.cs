using System.Text.Json;
using KLMN.Application.Common.Models;
using KLMN.Application.ICD10Diagnostics.Responses;

static void Check(bool value,string title)
{
    if(!value)
    {
        throw new InvalidOperationException(title);
    }
    Console.WriteLine("PASS "+title);
}

var id=Guid.NewGuid();
var icdId=Guid.NewGuid();
var response=new ICD10DiagnosticResponse(
    id,
    icdId,
    "A00.0",
    "Kolera",
    Guid.NewGuid(),
    "S01 — Tanı",
    Guid.NewGuid(),
    "Başvuru Grubu",
    7,
    true,
    1u,
    DateTime.UtcNow);

var serializerOptions=new JsonSerializerOptions(JsonSerializerDefaults.Web);

// Liste/ptable: API'deki camelCase web serializer ve gerçek PagedResult yapısını doğrular.
var result=new PagedResult<ICD10DiagnosticResponse>
{
    Items=[response],
    TotalCount=1,
    PageNumber=1,
    PageSize=20
};

using var listJson=JsonDocument.Parse(JsonSerializer.Serialize(result,serializerOptions));
var row=listJson.RootElement.GetProperty("items")[0];
Check(row.GetProperty("icd10Code").GetString()=="A00.0",
    "Liste p-table icd10Code alanını okuyabilir");
Check(row.GetProperty("icd10Name").GetString()=="Kolera",
    "Liste p-table icd10Name alanını okuyabilir");
Check(row.GetProperty("icd10DefId").GetGuid()==icdId,
    "Liste ve düzenleme icd10DefId alanını okuyabilir");
Check(!row.TryGetProperty("icD10Code",out _)
    &&!row.TryGetProperty("icD10Name",out _)
    &&!row.TryGetProperty("icD10DefId",out _),
    "Yanlış camelCase kısaltma alanları JSON'da bulunmaz");
Check(row.GetProperty("syyDiagnosisName").GetString()=="S01 — Tanı",
    "SYY Tanısı alanı korunur");
Check(row.GetProperty("hcAdmissionTypeName").GetString()=="Başvuru Grubu",
    "Başvuru Grubu alanı korunur");

// Detay: aynı response sözleşmesi güncelleme formunun ilişki ID'sini de korur.
using var detailJson=JsonDocument.Parse(JsonSerializer.Serialize(response,serializerOptions));
Check(detailJson.RootElement.GetProperty("icd10Code").GetString()=="A00.0",
    "Detay JSON'unda ICD10 Kodu vardır");
Check(detailJson.RootElement.GetProperty("icd10DefId").GetGuid()==icdId,
    "Detay JSON'unda ICD10 tanım ID'si vardır");
Console.WriteLine("ICD10 istatistikleri JSON regresyonları başarılı.");
