using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Procedures.Commands.Common;
using KLMN.Application.Procedures.Responses;
using KLMN.Domain.Entities.Admissions;
using MediatR;

namespace KLMN.Application.Procedures.Commands.CreateProcedureDef;

/// <summary>Tetkik Create iş akışını EF ve audit altyapısında yürütür.</summary>
public sealed class CreateProcedureDefCommandHandler(IKLMNDbContext db)
    : IRequestHandler<CreateProcedureDefCommand, ProcedureDefMutationResponse>
{
    /// <summary>Validasyonu yapılan isteği concurrency korumasıyla kaydeder.</summary>
    public async Task<ProcedureDefMutationResponse> Handle(CreateProcedureDefCommand request, CancellationToken ct)
    {
        await ProcedureDefRules.EnsureUniqueCodeAsync(db, request.ProcedureCode, null, ct);
        await ProcedureDefRules.EnsureTypeAsync(db,request.ProcedureType,ct);
        var item = new ProcedureDef
        {
            ProcedureCode = request.ProcedureCode.Trim().ToUpperInvariant(),
            Name = request.Name.Trim(),
            ValidFrom = request.ValidFrom,
            ValidTo = request.ValidTo,
            RequiresNumericResult = request.RequiresNumericResult,
            RequiresReferenceRange = request.RequiresReferenceRange,
            RequiresDeviationInfo = request.RequiresDeviationInfo,
            ProcedureType = request.ProcedureType
        };
        await db.ProcedureDefs.AddAsync(item, ct);
        await db.SaveChangesAsync(ct);
        return new ProcedureDefMutationResponse(item.Id, item.Version);
    }
}
