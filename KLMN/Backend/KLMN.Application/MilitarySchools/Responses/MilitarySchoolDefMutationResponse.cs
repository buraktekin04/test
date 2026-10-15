namespace KLMN.Application.MilitarySchools.Responses;

/// <summary>Kaydın yeni kimliği ve PostgreSQL xmin sürümünü döndürür.</summary>
public sealed record MilitarySchoolDefMutationResponse(Guid Id, uint Version);
