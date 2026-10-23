using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.ICD10Defs.Commands.Common;


using MediatR;
namespace KLMN.Application.ICD10Defs.Commands.DeleteICD10Def;
/// <summary>ICD10 Tanımları — Delete işlemi, soft-delete/audit/xmin mevcut altyapıda.</summary>
public sealed class DeleteICD10DefCommandHandler(IKLMNDbContext db):IRequestHandler<DeleteICD10DefCommand>
{
 public async Task Handle(DeleteICD10DefCommand r,CancellationToken ct)
 {
 var item=await ICD10DefRules.LoadAsync(db,r.Id,r.Version,ct);
 await ICD10DefRules.EnsureUnusedAsync(db,item.Id,false,ct);
 db.ICD10Defs.Remove(item);
 await db.SaveChangesAsync(ct);
 }
}
