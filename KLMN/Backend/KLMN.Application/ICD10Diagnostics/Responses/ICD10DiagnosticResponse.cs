using System.Text.Json.Serialization;

namespace KLMN.Application.ICD10Diagnostics.Responses;

/// <summary>
/// ICD10 SYY İstatistikleri liste ve detay API DTO'su.
/// ICD10 ile başlayan CLR özelliklerinin otomatik camelCase dönüşümü
/// (örn. icD10Code) Angular tarafındaki icd10Code ile uyuşmayabilir.
/// Açık JSON adları frontend tablo, sıralama ve düzenleme sözleşmesini korur.
/// </summary>
public sealed record ICD10DiagnosticResponse(
    [property: JsonPropertyName("id")] Guid Id,
    [property: JsonPropertyName("icd10DefId")] Guid ICD10DefId,
    [property: JsonPropertyName("icd10Code")] string ICD10Code,
    [property: JsonPropertyName("icd10Name")] string ICD10Name,
    [property: JsonPropertyName("syyDiagnosisDefId")] Guid SYYDiagnosisDefId,
    [property: JsonPropertyName("syyDiagnosisName")] string SYYDiagnosisName,
    [property: JsonPropertyName("hcAdmissionTypeDefId")] Guid HcAdmissionTypeDefId,
    [property: JsonPropertyName("hcAdmissionTypeName")] string HcAdmissionTypeName,
    [property: JsonPropertyName("amount")] int Amount,
    [property: JsonPropertyName("isActive")] bool IsActive,
    [property: JsonPropertyName("version")] uint Version,
    [property: JsonPropertyName("createdDate")] DateTime CreatedDate
);
