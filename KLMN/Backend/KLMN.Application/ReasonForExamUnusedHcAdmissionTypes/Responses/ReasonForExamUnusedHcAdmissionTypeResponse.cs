using KLMN.Domain.Entities.Admissions;
namespace KLMN.Application.ReasonForExamUnusedHcAdmissionTypes.Responses;
/// <summary>Kullanılmayacak Hasta Grupları için ana sebep ve bağlı referans adlarını da gösteren DTO.</summary>
public sealed record ReasonForExamUnusedHcAdmissionTypeResponse(
 Guid Id,Guid ReasonForExaminationDefId,string ReasonDescription,string ReasonCode,
 Guid ReferenceId,string ReferenceName,bool IsActive,uint Version,DateTime CreatedDate
 )
{
 /// <summary>Sunucu tarafında sunulacak alanlara güvenli EF projeksiyonu.</summary>

}
