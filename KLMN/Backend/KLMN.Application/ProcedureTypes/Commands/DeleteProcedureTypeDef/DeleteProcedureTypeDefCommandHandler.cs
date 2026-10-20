using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.ProcedureTypes.Commands.Common;


using MediatR;
namespace KLMN.Application.ProcedureTypes.Commands.DeleteProcedureTypeDef;
/// <summary>Tetkik türü Delete işlemini EF audit ve optimistic concurrency ile yürütür.</summary>
public sealed class DeleteProcedureTypeDefCommandHandler(IKLMNDbContext db):
    IRequestHandler<DeleteProcedureTypeDefCommand>
{
    public async Task Handle(DeleteProcedureTypeDefCommand r,CancellationToken ct)
    {
        var item=await ProcedureTypeRules.LoadAsync(db,r.Id,r.Version,ct);
        await ProcedureTypeRules.EnsureNoProceduresAsync(db,item.Code,ct);
        db.ProcedureTypeDefs.Remove(item);
        await db.SaveChangesAsync(ct);
    }
}
