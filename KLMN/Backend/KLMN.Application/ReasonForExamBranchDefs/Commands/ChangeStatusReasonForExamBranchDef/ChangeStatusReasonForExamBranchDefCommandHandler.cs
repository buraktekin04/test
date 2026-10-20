using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.ReasonForExamBranchDefs.Commands.Common;
using KLMN.Application.ReasonForExamBranchDefs.Responses;

using MediatR;
namespace KLMN.Application.ReasonForExamBranchDefs.Commands.ChangeStatusReasonForExamBranchDef;
/// <summary>Muayene Gereken Branşlar: ChangeStatus işlemi. Soft delete, audit ve xmin ortak DbContext'te uygulanır.</summary>
public sealed class ChangeStatusReasonForExamBranchDefCommandHandler(IKLMNDbContext db):IRequestHandler<ChangeStatusReasonForExamBranchDefCommand,ReasonForExamBranchDefMutationResponse>
{
 public async Task<ReasonForExamBranchDefMutationResponse> Handle(ChangeStatusReasonForExamBranchDefCommand r,CancellationToken ct)
 {
 var item=await ReasonForExamBranchDefRules.LoadAsync(db,r.Id,r.Version,ct);
 if(r.IsActive) await ReasonForExamBranchDefRules.EnsureReferencesAsync(db,item.ReasonForExaminationDefId,item.BranchDefId,ct);
 item.IsActive=r.IsActive;
 await db.SaveChangesAsync(ct);
 return new(item.Id,item.Version);
 }
}
