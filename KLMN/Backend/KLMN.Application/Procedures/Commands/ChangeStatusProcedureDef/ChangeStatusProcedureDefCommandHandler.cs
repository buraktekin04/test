using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Procedures.Commands.Common;
using KLMN.Application.Procedures.Responses;
using KLMN.Application.Procedures.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.Procedures.Commands.ChangeStatusProcedureDef;
/// <summary>Gereken tetkik ilişkileri varken pasifleştirmeyi önler; xmin kontrolünü korur.</summary>
public sealed class ChangeStatusProcedureDefCommandHandler(IKLMNDbContext db)
 : IRequestHandler<ChangeStatusProcedureDefCommand,ProcedureDefMutationResponse>
{
 public async Task<ProcedureDefMutationResponse> Handle(ChangeStatusProcedureDefCommand r,CancellationToken ct)
 {
 var item=await ProcedureDefRules.LoadForChangeAsync(db,r.Id,r.Version,ct);
 if(!r.IsActive&&await db.ReasonForExamProcedureDefs.IgnoreQueryFilters()
    .AnyAsync(x=>!x.IsDeleted&&x.IsActive&&x.ProcedureDefId==item.Id,ct))
   throw new ProcedureConflictException("Aktif muayene sebebi eşleştirmesi bulunan tetkik pasifleştirilemez.");
 item.IsActive=r.IsActive;await db.SaveChangesAsync(ct);
 return new ProcedureDefMutationResponse(item.Id,item.Version);
 }
}
