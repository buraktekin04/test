using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.ReportTypes.Commands.Common;
using KLMN.Application.ReportTypes.Responses;
using MediatR;

namespace KLMN.Application.ReportTypes.Commands.UpdateReportTypeDef;

/// <summary>Rapor Türü Update iş akışını EF ve audit altyapısında yürütür.</summary>
public sealed class UpdateReportTypeDefCommandHandler(IKLMNDbContext db)
    : IRequestHandler<UpdateReportTypeDefCommand, ReportTypeDefMutationResponse>
{
    /// <summary>Validasyonu yapılan isteği concurrency korumasıyla kaydeder.</summary>
    public async Task<ReportTypeDefMutationResponse> Handle(UpdateReportTypeDefCommand request, CancellationToken ct)
    {
        var item = await ReportTypeDefRules.LoadForChangeAsync(db, request.Id, request.Version, ct);
        await ReportTypeDefRules.EnsureUniqueCodeAsync(db, request.Code, item.Id, ct);
        item.Code = request.Code.Trim().ToUpperInvariant();
        item.Name = request.Name.Trim();
        item.ValidFrom = request.ValidFrom;
        item.ValidTo = request.ValidTo;
        await db.SaveChangesAsync(ct);
        return new ReportTypeDefMutationResponse(item.Id, item.Version);
    }
}
