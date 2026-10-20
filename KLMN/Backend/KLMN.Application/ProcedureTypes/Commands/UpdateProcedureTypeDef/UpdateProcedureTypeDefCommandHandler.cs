using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.ProcedureTypes.Commands.Common;
using KLMN.Application.ProcedureTypes.Responses;

using MediatR;
namespace KLMN.Application.ProcedureTypes.Commands.UpdateProcedureTypeDef;
/// <summary>Tetkik türü Update işlemini EF audit ve optimistic concurrency ile yürütür.</summary>
public sealed class UpdateProcedureTypeDefCommandHandler(IKLMNDbContext db):
    IRequestHandler<UpdateProcedureTypeDefCommand,ProcedureTypeDefMutationResponse>
{
    public async Task<ProcedureTypeDefMutationResponse> Handle(UpdateProcedureTypeDefCommand r,CancellationToken ct)
    {
        var item=await ProcedureTypeRules.LoadAsync(db,r.Id,r.Version,ct);
        item.Name=r.Name.Trim(); // Enum uyumlu Code hiçbir zaman düzenlenmez.
        await db.SaveChangesAsync(ct);
        return new(item.Id,item.Version);
    }
}
