using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Branches.Commands.Common;
using KLMN.Application.Branches.Responses;
using KLMN.Domain.Entities.Admissions;
using MediatR;

namespace KLMN.Application.Branches.Commands.CreateBranchDef;

/// <summary>Branş Create iş akışını EF ve audit altyapısında yürütür.</summary>
public sealed class CreateBranchDefCommandHandler(IKLMNDbContext db)
    : IRequestHandler<CreateBranchDefCommand, BranchDefMutationResponse>
{
    /// <summary>Validasyonu yapılan isteği concurrency korumasıyla kaydeder.</summary>
    public async Task<BranchDefMutationResponse> Handle(CreateBranchDefCommand request, CancellationToken ct)
    {
        await BranchDefRules.EnsureUniqueCodeAsync(db, request.Code, null, ct);
        var item = new BranchDef
        {
            Code = request.Code.Trim().ToUpperInvariant(),
            Name = request.Name.Trim(),
            ValidFrom = request.ValidFrom,
            ValidTo = request.ValidTo
        };
        await db.BranchDefs.AddAsync(item, ct);
        await db.SaveChangesAsync(ct);
        return new BranchDefMutationResponse(item.Id, item.Version);
    }
}
