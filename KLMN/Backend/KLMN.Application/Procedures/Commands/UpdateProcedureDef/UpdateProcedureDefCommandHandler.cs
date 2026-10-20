using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Procedures.Commands.Common;
using KLMN.Application.Procedures.Responses;
using MediatR;

namespace KLMN.Application.Procedures.Commands.UpdateProcedureDef;

/// <summary>Tetkik Update iş akışını EF ve audit altyapısında yürütür.</summary>
public sealed class UpdateProcedureDefCommandHandler(IKLMNDbContext db)
    : IRequestHandler<UpdateProcedureDefCommand, ProcedureDefMutationResponse>
{
    /// <summary>Validasyonu yapılan isteği concurrency korumasıyla kaydeder.</summary>
    public async Task<ProcedureDefMutationResponse> Handle(UpdateProcedureDefCommand request, CancellationToken ct)
    {
        var item = await ProcedureDefRules.LoadForChangeAsync(db, request.Id, request.Version, ct);
        await ProcedureDefRules.EnsureUniqueCodeAsync(db, request.Code, item.Id, ct);
        item.Code = request.Code.Trim().ToUpperInvariant();
        item.Name = request.Name.Trim();
        item.ValidFrom = request.ValidFrom;
        item.ValidTo = request.ValidTo;
        await db.SaveChangesAsync(ct);
        return new ProcedureDefMutationResponse(item.Id, item.Version);
    }
}
