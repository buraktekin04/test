using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.EquivalentProcedures.Commands.Common;
using KLMN.Domain.Entities.Admissions;

using KLMN.Application.EquivalentProcedures.Responses;
using MediatR;
namespace KLMN.Application.EquivalentProcedures.Commands.CreateEquivalentProcedure;
/// <summary>FK, benzersizlik ve BaseEntity optimistic concurrency kuralları.</summary>
public sealed class CreateEquivalentProcedureCommandHandler(IKLMNDbContext db):IRequestHandler<CreateEquivalentProcedureCommand,EquivalentProcedureMutationResponse>
{
 public async Task<EquivalentProcedureMutationResponse> Handle(CreateEquivalentProcedureCommand r,CancellationToken ct)
 {
  var (left,right)=EquivalentProcedureRules.Normalize(r.ProcedureDefId,r.EquivalentProcedureId);
  await EquivalentProcedureRules.EnsureReferencesAsync(db,left,right,ct);
  await EquivalentProcedureRules.EnsureUniqueAsync(db,left,right,null,ct);
  var item=new EquivalentProcedureDef{ProcedureDefId=left,EquivalentProcedureId=right};
  await db.EquivalentProcedureDefs.AddAsync(item,ct);
  await db.SaveChangesAsync(ct);return new(item.Id,item.Version);
 }
}
