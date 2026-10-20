using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.ReasonForExamBranchDefs.Commands.Common;
using KLMN.Application.ReasonForExamBranchDefs.Responses;
using KLMN.Domain.Entities.Admissions;
using MediatR;
namespace KLMN.Application.ReasonForExamBranchDefs.Commands.CreateReasonForExamBranchDef;
/// <summary>Muayene Gereken Branşlar: Create işlemi. Soft delete, audit ve xmin ortak DbContext'te uygulanır.</summary>
public sealed class CreateReasonForExamBranchDefCommandHandler(IKLMNDbContext db):IRequestHandler<CreateReasonForExamBranchDefCommand,ReasonForExamBranchDefMutationResponse>
{
 public async Task<ReasonForExamBranchDefMutationResponse> Handle(CreateReasonForExamBranchDefCommand r,CancellationToken ct)
 {
 await ReasonForExamBranchDefRules.EnsureReferencesAsync(db,r.ReasonForExaminationDefId,r.ReferenceId,ct);
 await ReasonForExamBranchDefRules.EnsureUniqueAsync(db,r.ReasonForExaminationDefId,r.ReferenceId,r.StartingAge,r.EndingAge,r.Gender,null,ct);
 var item=new ReasonForExamBranchDef{ReasonForExaminationDefId=r.ReasonForExaminationDefId,BranchDefId=r.ReferenceId,StartingAge=r.StartingAge,EndingAge=r.EndingAge,Gender=r.Gender};
 await db.ReasonForExamBranchDefs.AddAsync(item,ct);
 await db.SaveChangesAsync(ct);
 return new(item.Id,item.Version);
 }
}
