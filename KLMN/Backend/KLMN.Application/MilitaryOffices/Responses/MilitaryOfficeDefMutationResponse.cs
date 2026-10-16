namespace KLMN.Application.MilitaryOffices.Responses;

/// <summary>Şube kimliği ve PostgreSQL xmin sürümü.</summary>
public sealed record MilitaryOfficeDefMutationResponse(Guid Id, uint Version);
