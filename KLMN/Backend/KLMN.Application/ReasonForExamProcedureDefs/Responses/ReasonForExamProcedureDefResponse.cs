using System.Linq.Expressions;
using KLMN.Domain.Entities.Admissions;
namespace KLMN.Application.ReasonForExamProcedureDefs.Responses;
/// <summary>Gereken Tetkikler için ana sebep ve bağlı referans adlarını da gösteren DTO.</summary>
public sealed record ReasonForExamProcedureDefResponse(
 Guid Id,Guid ReasonForExaminationDefId,string ReasonDescription,string ReasonCode,
 Guid ReferenceId,string ReferenceName,bool IsActive,uint Version,DateTime CreatedDate
 ,int StartingAge,int EndingAge,int Gender)
{
 /// <summary>Sunucu tarafında sunulacak alanlara güvenli EF projeksiyonu.</summary>
 public static Expression<Func<ReasonForExamProcedureDef,ReasonForExamProcedureDefResponse>> Projection => x=>new ReasonForExamProcedureDefResponse(
 x.Id,x.ReasonForExaminationDefId,x.Reason.Description,x.Reason.Code,
 x.ProcedureDefId,x.Procedure.Name,x.IsActive,x.Version,x.CreatedDate
 ,x.StartingAge,x.EndingAge,x.Gender);
}
