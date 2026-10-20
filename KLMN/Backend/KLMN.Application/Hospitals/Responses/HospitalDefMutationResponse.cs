namespace KLMN.Application.Hospitals.Responses;

/// <summary>Kaydedilen sağlık tesisinin kimliği ve PostgreSQL xmin değeri.</summary>
public sealed record HospitalDefMutationResponse(Guid Id, uint Version);
