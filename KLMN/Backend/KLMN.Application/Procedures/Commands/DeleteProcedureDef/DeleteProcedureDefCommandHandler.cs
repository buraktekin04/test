using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Procedures.Commands.Common;
using KLMN.Application.Procedures.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.Procedures.Commands.DeleteProcedureDef;
/// <summary>Silinmemiş muayene sebebi ilişkisi olan tetkiğin silinmesini engeller.</summary>
public sealed class DeleteProcedureDefCommandHandler(IKLMNDbContext db):IRequestHandler<DeleteProcedureDefCommand>
{
 public async Task Handle(DeleteProcedureDefCommand r,CancellationToken ct)
 {
 var item=await ProcedureDefRules.LoadForChangeAsync(db,r.Id,r.Version,ct);
 if(await db.ReasonForExamProcedureDefs.IgnoreQueryFilters()
     .AnyAsync(x=>!x.IsDeleted&&x.ProcedureDefId==item.Id,ct))
   throw new ProcedureConflictException("Muayene sebebi eşleştirmesi bulunan tetkik silinemez.");
 if(await db.EquivalentProcedureDefs.IgnoreQueryFilters()
    .AnyAsync(x=>!x.IsDeleted&&(x.ProcedureDefId==item.Id||x.EquivalentProcedureId==item.Id),ct))
   throw new ProcedureConflictException("Eşdeğer tetkik eşleştirmesi bulunan tetkik silinemez.");
 db.ProcedureDefs.Remove(item);await db.SaveChangesAsync(ct);
 }
}
