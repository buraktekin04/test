using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.ReasonForExamProcedureDefs.Commands.Common;
using KLMN.Application.ReasonForExamProcedureDefs.Responses;

using MediatR;
namespace KLMN.Application.ReasonForExamProcedureDefs.Commands.UpdateReasonForExamProcedureDef;
/// <summary>Gereken Tetkikler: Update işlemi. Soft delete, audit ve xmin ortak DbContext'te uygulanır.</summary>
public sealed class UpdateReasonForExamProcedureDefCommandHandler(IKLMNDbContext db):IRequestHandler<UpdateReasonForExamProcedureDefCommand,ReasonForExamProcedureDefMutationResponse>
{
 public async Task<ReasonForExamProcedureDefMutationResponse> Handle(UpdateReasonForExamProcedureDefCommand r,CancellationToken ct)
 {
 var item=await ReasonForExamProcedureDefRules.LoadAsync(db,r.Id,r.Version,ct);
 await ReasonForExamProcedureDefRules.EnsureReferencesAsync(db,r.ReasonForExaminationDefId,r.ReferenceId,ct,item.ReasonForExaminationDefId,item.ProcedureDefId);
 await ReasonForExamProcedureDefRules.EnsureUniqueAsync(db,r.ReasonForExaminationDefId,r.ReferenceId,r.StartingAge,r.EndingAge,r.Gender,item.Id,ct);
 item.ReasonForExaminationDefId=r.ReasonForExaminationDefId;item.ProcedureDefId=r.ReferenceId;
 item.StartingAge=r.StartingAge;item.EndingAge=r.EndingAge;item.Gender=r.Gender;
 await db.SaveChangesAsync(ct);
 return new(item.Id,item.Version);
 }
}
