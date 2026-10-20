namespace KLMN.Application.ReasonForExaminations.Responses;
/// <summary>Kayıt kimliği ve PostgreSQL xmin sürümü.</summary>
public sealed record ReasonForExaminationDefMutationResponse(Guid Id,uint Version);
