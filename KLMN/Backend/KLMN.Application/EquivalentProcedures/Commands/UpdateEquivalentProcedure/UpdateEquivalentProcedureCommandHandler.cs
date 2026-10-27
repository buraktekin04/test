using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.EquivalentProcedures.Commands.Common;


using KLMN.Application.EquivalentProcedures.Responses;
using MediatR;
namespace KLMN.Application.EquivalentProcedures.Commands.UpdateEquivalentProcedure;
/// <summary>FK, benzersizlik ve BaseEntity optimistic concurrency kuralları.</summary>
public sealed class UpdateEquivalentProcedureCommandHandler(IKLMNDbContext db):IRequestHandler<UpdateEquivalentProcedureCommand,EquivalentProcedureMutationResponse>
{
 public async Task<EquivalentProcedureMutationResponse> Handle(UpdateEquivalentProcedureCommand r,CancellationToken ct)
 {
  var item=await EquivalentProcedureRules.LoadAsync(db,r.Id,r.Version,ct);
  var (left,right)=EquivalentProcedureRules.Normalize(r.ProcedureDefId,r.EquivalentProcedureId);
  await EquivalentProcedureRules.EnsureReferencesAsync(db,left,right,ct,item);
  await EquivalentProcedureRules.EnsureUniqueAsync(db,left,right,item.Id,ct);
  item.ProcedureDefId=left;item.EquivalentProcedureId=right;
  await db.SaveChangesAsync(ct);return new(item.Id,item.Version);
 }
}
