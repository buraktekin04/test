using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.ReasonForExamBranchDefs.Commands.Common;
using KLMN.Application.ReasonForExamBranchDefs.Responses;

using MediatR;
namespace KLMN.Application.ReasonForExamBranchDefs.Commands.UpdateReasonForExamBranchDef;
/// <summary>Muayene Gereken Branşlar: Update işlemi. Soft delete, audit ve xmin ortak DbContext'te uygulanır.</summary>
public sealed class UpdateReasonForExamBranchDefCommandHandler(IKLMNDbContext db):IRequestHandler<UpdateReasonForExamBranchDefCommand,ReasonForExamBranchDefMutationResponse>
{
 public async Task<ReasonForExamBranchDefMutationResponse> Handle(UpdateReasonForExamBranchDefCommand r,CancellationToken ct)
 {
 var item=await ReasonForExamBranchDefRules.LoadAsync(db,r.Id,r.Version,ct);
 await ReasonForExamBranchDefRules.EnsureReferencesAsync(db,r.ReasonForExaminationDefId,r.ReferenceId,ct,item.ReasonForExaminationDefId,item.BranchDefId);
 await ReasonForExamBranchDefRules.EnsureUniqueAsync(db,r.ReasonForExaminationDefId,r.ReferenceId,r.StartingAge,r.EndingAge,r.Gender,item.Id,ct);
 item.ReasonForExaminationDefId=r.ReasonForExaminationDefId;item.BranchDefId=r.ReferenceId;
 item.StartingAge=r.StartingAge;item.EndingAge=r.EndingAge;item.Gender=r.Gender;
 await db.SaveChangesAsync(ct);
 return new(item.Id,item.Version);
 }
}
