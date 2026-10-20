using System.Linq.Expressions;
using KLMN.Domain.Entities.Admissions;
namespace KLMN.Application.ReasonForExamBranchDefs.Responses;
/// <summary>Muayene Gereken Branşlar için ana sebep ve bağlı referans adlarını da gösteren DTO.</summary>
public sealed record ReasonForExamBranchDefResponse(
 Guid Id,Guid ReasonForExaminationDefId,string ReasonDescription,string ReasonCode,
 Guid ReferenceId,string ReferenceName,bool IsActive,uint Version,DateTime CreatedDate
 ,int StartingAge,int EndingAge,int Gender)
{
 /// <summary>Sunucu tarafında sunulacak alanlara güvenli EF projeksiyonu.</summary>
 public static Expression<Func<ReasonForExamBranchDef,ReasonForExamBranchDefResponse>> Projection => x=>new ReasonForExamBranchDefResponse(
 x.Id,x.ReasonForExaminationDefId,x.Reason.Description,x.Reason.Code,
 x.BranchDefId,x.Branch.Name,x.IsActive,x.Version,x.CreatedDate
 ,x.StartingAge,x.EndingAge,x.Gender);
}
