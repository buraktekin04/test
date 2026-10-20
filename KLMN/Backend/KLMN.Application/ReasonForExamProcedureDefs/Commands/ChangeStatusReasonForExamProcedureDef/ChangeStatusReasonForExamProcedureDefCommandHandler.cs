using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.ReasonForExamProcedureDefs.Commands.Common;
using KLMN.Application.ReasonForExamProcedureDefs.Responses;

using MediatR;
namespace KLMN.Application.ReasonForExamProcedureDefs.Commands.ChangeStatusReasonForExamProcedureDef;
/// <summary>Gereken Tetkikler: ChangeStatus işlemi. Soft delete, audit ve xmin ortak DbContext'te uygulanır.</summary>
public sealed class ChangeStatusReasonForExamProcedureDefCommandHandler(IKLMNDbContext db):IRequestHandler<ChangeStatusReasonForExamProcedureDefCommand,ReasonForExamProcedureDefMutationResponse>
{
 public async Task<ReasonForExamProcedureDefMutationResponse> Handle(ChangeStatusReasonForExamProcedureDefCommand r,CancellationToken ct)
 {
 var item=await ReasonForExamProcedureDefRules.LoadAsync(db,r.Id,r.Version,ct);
 if(r.IsActive) await ReasonForExamProcedureDefRules.EnsureReferencesAsync(db,item.ReasonForExaminationDefId,item.ProcedureDefId,ct);
 item.IsActive=r.IsActive;
 await db.SaveChangesAsync(ct);
 return new(item.Id,item.Version);
 }
}
