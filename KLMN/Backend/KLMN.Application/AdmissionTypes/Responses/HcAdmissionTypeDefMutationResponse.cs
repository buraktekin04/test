namespace KLMN.Application.AdmissionTypes.Responses;

/// <summary>Başvuru grubu kimliği ve PostgreSQL xmin sürüm bilgisi.</summary>
public sealed record HcAdmissionTypeDefMutationResponse(Guid Id, uint Version);
