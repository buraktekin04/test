using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.ICD10Defs.Commands.Common;
using KLMN.Application.ICD10Defs.Responses;

using MediatR;
namespace KLMN.Application.ICD10Defs.Commands.ChangeStatusICD10Def;
/// <summary>ICD10 Tanımları — ChangeStatus işlemi, soft-delete/audit/xmin mevcut altyapıda.</summary>
public sealed class ChangeStatusICD10DefCommandHandler(IKLMNDbContext db):IRequestHandler<ChangeStatusICD10DefCommand,ICD10DefMutationResponse>
{
 public async Task<ICD10DefMutationResponse> Handle(ChangeStatusICD10DefCommand r,CancellationToken ct)
 {
 var item=await ICD10DefRules.LoadAsync(db,r.Id,r.Version,ct);
 if(!r.IsActive)await ICD10DefRules.EnsureUnusedAsync(db,item.Id,true,ct);
 item.IsActive=r.IsActive;
 await db.SaveChangesAsync(ct);return new(item.Id,item.Version);
 }
}
