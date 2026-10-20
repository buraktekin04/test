namespace KLMN.Application.HospitalReportTypes.Responses;

/// <summary>Kaydedilen tesis–rapor türü eşleştirmesinin kimliği ve PostgreSQL xmin değeri.</summary>
public sealed record HospitalReportTypeMutationResponse(Guid Id, uint Version);
