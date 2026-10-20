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
        await ProcedureDefRules.EnsureUniqueCodeAsync(db, request.ProcedureCode, item.Id, ct);
        // Değişen tür seçimi, aktif dinamik tür tablosunda doğrulanır.
        if(item.ProcedureType!=request.ProcedureType)
            await ProcedureDefRules.EnsureTypeAsync(db,request.ProcedureType,ct);
        item.ProcedureCode = request.ProcedureCode.Trim().ToUpperInvariant();
        item.Name = request.Name.Trim();
        item.StartDate = request.StartDate;
        item.EndDate = request.EndDate;
        item.RequiresNumericResult = request.RequiresNumericResult;
        item.RequiresReferenceRange = request.RequiresReferenceRange;
        item.RequiresDeviationInfo = request.RequiresDeviationInfo;
        item.ProcedureType = request.ProcedureType;
        await db.SaveChangesAsync(ct);
        return new ProcedureDefMutationResponse(item.Id, item.Version);
    }
}
