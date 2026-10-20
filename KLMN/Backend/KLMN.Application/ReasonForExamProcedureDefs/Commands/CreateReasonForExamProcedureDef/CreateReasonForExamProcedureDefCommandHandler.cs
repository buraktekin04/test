using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.ReasonForExamProcedureDefs.Commands.Common;
using KLMN.Application.ReasonForExamProcedureDefs.Responses;
using KLMN.Domain.Entities.Admissions;
using MediatR;
namespace KLMN.Application.ReasonForExamProcedureDefs.Commands.CreateReasonForExamProcedureDef;
/// <summary>Gereken Tetkikler: Create işlemi. Soft delete, audit ve xmin ortak DbContext'te uygulanır.</summary>
public sealed class CreateReasonForExamProcedureDefCommandHandler(IKLMNDbContext db):IRequestHandler<CreateReasonForExamProcedureDefCommand,ReasonForExamProcedureDefMutationResponse>
{
 public async Task<ReasonForExamProcedureDefMutationResponse> Handle(CreateReasonForExamProcedureDefCommand r,CancellationToken ct)
 {
 await ReasonForExamProcedureDefRules.EnsureReferencesAsync(db,r.ReasonForExaminationDefId,r.ReferenceId,ct);
 await ReasonForExamProcedureDefRules.EnsureUniqueAsync(db,r.ReasonForExaminationDefId,r.ReferenceId,r.StartingAge,r.EndingAge,r.Gender,null,ct);
 var item=new ReasonForExamProcedureDef{ReasonForExaminationDefId=r.ReasonForExaminationDefId,ProcedureDefId=r.ReferenceId,StartingAge=r.StartingAge,EndingAge=r.EndingAge,Gender=r.Gender};
 await db.ReasonForExamProcedureDefs.AddAsync(item,ct);
 await db.SaveChangesAsync(ct);
 return new(item.Id,item.Version);
 }
}
