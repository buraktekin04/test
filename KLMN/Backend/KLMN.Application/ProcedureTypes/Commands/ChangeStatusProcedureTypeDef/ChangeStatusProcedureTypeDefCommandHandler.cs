using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.ProcedureTypes.Commands.Common;
using KLMN.Application.ProcedureTypes.Responses;

using MediatR;
namespace KLMN.Application.ProcedureTypes.Commands.ChangeStatusProcedureTypeDef;
/// <summary>Tetkik türü ChangeStatus işlemini EF audit ve optimistic concurrency ile yürütür.</summary>
public sealed class ChangeStatusProcedureTypeDefCommandHandler(IKLMNDbContext db):
    IRequestHandler<ChangeStatusProcedureTypeDefCommand,ProcedureTypeDefMutationResponse>
{
    public async Task<ProcedureTypeDefMutationResponse> Handle(ChangeStatusProcedureTypeDefCommand r,CancellationToken ct)
    {
        var item=await ProcedureTypeRules.LoadAsync(db,r.Id,r.Version,ct);
        if(!r.IsActive)await ProcedureTypeRules.EnsureNoActiveProceduresAsync(db,item.Id,ct);
        item.IsActive=r.IsActive;
        await db.SaveChangesAsync(ct);
        return new(item.Id,item.Version);
    }
}
