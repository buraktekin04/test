namespace KLMN.Application.SYYDiagnosisDecisions.Responses;
/// <summary>Karar istatistiği tablo verisi; görüntü adları EF sorgusu içinde doldurulur.</summary>
public sealed record SYYDiagnosisDecisionResponse(
 Guid Id,Guid SYYDiagnosisDefId,string SYYDiagnosisCode,string SYYDiagnosisName,
 Guid HcDecisionDefId,int HcDecisionCode,string HcDecisionName,
 Guid HcAdmissionTypeDefId,string HcAdmissionTypeName,
 Guid? MilitaryClassDefId,string? MilitaryClassName,int Amount,
 bool IsActive,uint Version,DateTime CreatedDate);
