using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.ReportTypes.Commands.Common;
using KLMN.Application.ReportTypes.Responses;
using MediatR;

namespace KLMN.Application.ReportTypes.Commands.ChangeStatusReportTypeDef;

/// <summary>Rapor Türü ChangeStatus iş akışını EF ve audit altyapısında yürütür.</summary>
public sealed class ChangeStatusReportTypeDefCommandHandler(IKLMNDbContext db)
    : IRequestHandler<ChangeStatusReportTypeDefCommand, ReportTypeDefMutationResponse>
{
    /// <summary>Validasyonu yapılan isteği concurrency korumasıyla kaydeder.</summary>
    public async Task<ReportTypeDefMutationResponse> Handle(ChangeStatusReportTypeDefCommand request, CancellationToken ct)
    {
        var item = await ReportTypeDefRules.LoadForChangeAsync(db, request.Id, request.Version, ct);
        item.IsActive = request.IsActive;
        await db.SaveChangesAsync(ct);
        return new ReportTypeDefMutationResponse(item.Id, item.Version);
    }
}
