using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.ReportTypes.Commands.Common;
using MediatR;

namespace KLMN.Application.ReportTypes.Commands.DeleteReportTypeDef;

/// <summary>Rapor Türü Delete iş akışını EF ve audit altyapısında yürütür.</summary>
public sealed class DeleteReportTypeDefCommandHandler(IKLMNDbContext db)
    : IRequestHandler<DeleteReportTypeDefCommand>
{
    /// <summary>Validasyonu yapılan isteği concurrency korumasıyla kaydeder.</summary>
    public async Task Handle(DeleteReportTypeDefCommand request, CancellationToken ct)
    {
        var item = await ReportTypeDefRules.LoadForChangeAsync(db, request.Id, request.Version, ct);
        db.ReportTypeDefs.Remove(item);
        await db.SaveChangesAsync(ct);
    }
}
