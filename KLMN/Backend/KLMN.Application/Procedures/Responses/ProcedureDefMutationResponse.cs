namespace KLMN.Application.Procedures.Responses;

/// <summary>Kaydedilen tetkikın kimliği ve PostgreSQL xmin değeri.</summary>
public sealed record ProcedureDefMutationResponse(Guid Id, uint Version);
