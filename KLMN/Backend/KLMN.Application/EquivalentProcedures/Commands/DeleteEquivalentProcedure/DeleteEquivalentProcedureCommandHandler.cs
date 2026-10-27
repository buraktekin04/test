using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.EquivalentProcedures.Commands.Common;



using MediatR;
namespace KLMN.Application.EquivalentProcedures.Commands.DeleteEquivalentProcedure;
/// <summary>FK, benzersizlik ve BaseEntity optimistic concurrency kuralları.</summary>
public sealed class DeleteEquivalentProcedureCommandHandler(IKLMNDbContext db):IRequestHandler<DeleteEquivalentProcedureCommand>
{
 public async Task Handle(DeleteEquivalentProcedureCommand r,CancellationToken ct)
 {
  var item=await EquivalentProcedureRules.LoadAsync(db,r.Id,r.Version,ct);
  db.EquivalentProcedureDefs.Remove(item);await db.SaveChangesAsync(ct);
 }
}
