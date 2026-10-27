using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.EquivalentProcedures.Commands.Common;

using KLMN.Application.EquivalentProcedures.Exceptions;
using Microsoft.EntityFrameworkCore;
using KLMN.Application.EquivalentProcedures.Responses;
using MediatR;
namespace KLMN.Application.EquivalentProcedures.Commands.ChangeStatusEquivalentProcedure;
/// <summary>FK, benzersizlik ve BaseEntity optimistic concurrency kuralları.</summary>
public sealed class ChangeStatusEquivalentProcedureCommandHandler(IKLMNDbContext db):IRequestHandler<ChangeStatusEquivalentProcedureCommand,EquivalentProcedureMutationResponse>
{
 public async Task<EquivalentProcedureMutationResponse> Handle(ChangeStatusEquivalentProcedureCommand r,CancellationToken ct)
 {
  var item=await EquivalentProcedureRules.LoadAsync(db,r.Id,r.Version,ct);
  if(r.IsActive){
    if(!await db.ProcedureDefs.AnyAsync(x=>x.Id==item.ProcedureDefId&&x.IsActive,ct)||
       !await db.ProcedureDefs.AnyAsync(x=>x.Id==item.EquivalentProcedureId&&x.IsActive,ct))
      throw new EquivalentProcedureConflictException("Pasif tetkik içeren eşleştirme aktifleştirilemez.");
  }
  item.IsActive=r.IsActive;
  await db.SaveChangesAsync(ct);return new(item.Id,item.Version);
 }
}
