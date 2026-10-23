using KLMN.Domain.Entities.Admissions;
namespace KLMN.Application.ReasonForExamReportTypes.Responses;
/// <summary>Muayene Sebebi Rapor Türleri için ana sebep ve bağlı referans adlarını da gösteren DTO.</summary>
public sealed record ReasonForExamReportTypeResponse(
 Guid Id,Guid ReasonForExaminationDefId,string ReasonDescription,string ReasonCode,
 Guid ReferenceId,string ReferenceName,bool IsActive,uint Version,DateTime CreatedDate
 )
{
 /// <summary>Sunucu tarafında sunulacak alanlara güvenli EF projeksiyonu.</summary>

}
