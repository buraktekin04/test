namespace KLMN.Application.ReportTypes.Responses;

/// <summary>Kaydedilen rapor türüın kimliği ve PostgreSQL xmin değeri.</summary>
public sealed record ReportTypeDefMutationResponse(Guid Id, uint Version);
