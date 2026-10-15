namespace KLMN.Application.MilitaryClasses.Responses;

/// <summary>Mutasyon sonrası kimlik ve PostgreSQL xmin sürümünü verir.</summary>
public sealed record MilitaryClassDefMutationResponse(Guid Id, uint Version);
