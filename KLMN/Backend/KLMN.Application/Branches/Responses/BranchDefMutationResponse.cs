namespace KLMN.Application.Branches.Responses;

/// <summary>Kaydedilen branşın kimliği ve PostgreSQL xmin değeri.</summary>
public sealed record BranchDefMutationResponse(Guid Id, uint Version);
