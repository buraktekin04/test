using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.ProcedureTypes.Commands.Common;
using KLMN.Application.ProcedureTypes.Responses;
using KLMN.Domain.Entities.Admissions;
using MediatR;
namespace KLMN.Application.ProcedureTypes.Commands.CreateProcedureTypeDef;
/// <summary>Tetkik türü Create işlemini EF audit ve optimistic concurrency ile yürütür.</summary>
public sealed class CreateProcedureTypeDefCommandHandler(IKLMNDbContext db):
    IRequestHandler<CreateProcedureTypeDefCommand,ProcedureTypeDefMutationResponse>
{
    public async Task<ProcedureTypeDefMutationResponse> Handle(CreateProcedureTypeDefCommand r,CancellationToken ct)
    {
        await ProcedureTypeRules.EnsureNewCodeAsync(db,r.Code,ct);
        var item=new ProcedureTypeDef{Code=r.Code,Name=r.Name.Trim()};
        await db.ProcedureTypeDefs.AddAsync(item,ct);
        await db.SaveChangesAsync(ct);
        return new(item.Id,item.Version);
    }
}
