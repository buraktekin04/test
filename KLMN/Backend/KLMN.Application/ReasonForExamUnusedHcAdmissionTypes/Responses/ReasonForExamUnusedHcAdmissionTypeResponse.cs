using System.Linq.Expressions;
using KLMN.Domain.Entities.Admissions;
namespace KLMN.Application.ReasonForExamUnusedHcAdmissionTypes.Responses;
/// <summary>Kullanılmayacak Hasta Grupları için ana sebep ve bağlı referans adlarını da gösteren DTO.</summary>
public sealed record ReasonForExamUnusedHcAdmissionTypeResponse(
 Guid Id,Guid ReasonForExaminationDefId,string ReasonDescription,string ReasonCode,
 Guid ReferenceId,string ReferenceName,bool IsActive,uint Version,DateTime CreatedDate
 )
{
 /// <summary>Sunucu tarafında sunulacak alanlara güvenli EF projeksiyonu.</summary>
 public static Expression<Func<ReasonForExamUnusedHcAdmissionType,ReasonForExamUnusedHcAdmissionTypeResponse>> Projection => x=>new ReasonForExamUnusedHcAdmissionTypeResponse(
 x.Id,x.ReasonForExaminationDefId,x.Reason.Description,x.Reason.Code,
 x.HcAdmissionTypeDefId,x.AdmissionType.Name,x.IsActive,x.Version,x.CreatedDate
 );
}
