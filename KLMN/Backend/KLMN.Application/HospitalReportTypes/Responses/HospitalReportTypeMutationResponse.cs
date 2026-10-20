namespace KLMN.Application.HospitalReportTypes.Responses;

/// <summary>Kaydedilen sağlık tesisi rapor türüın kimliği ve PostgreSQL xmin değeri.</summary>
public sealed record HospitalReportTypeMutationResponse(Guid Id, uint Version);
