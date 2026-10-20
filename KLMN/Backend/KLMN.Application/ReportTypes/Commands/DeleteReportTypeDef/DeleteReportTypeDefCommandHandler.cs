using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.ReportTypes.Commands.Common;
using MediatR;
using KLMN.Application.ReportTypes.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.ReportTypes.Commands.DeleteReportTypeDef;

/// <summary>Rapor Türü Delete iş akışını EF ve audit altyapısında yürütür.</summary>
public sealed class DeleteReportTypeDefCommandHandler(IKLMNDbContext db)
    : IRequestHandler<DeleteReportTypeDefCommand>
{
    /// <summary>Validasyonu yapılan isteği concurrency korumasıyla kaydeder.</summary>
    public async Task Handle(DeleteReportTypeDefCommand request, CancellationToken ct)
    {
        var item = await ReportTypeDefRules.LoadForChangeAsync(db, request.Id, request.Version, ct);
        if (await db.HospitalReportTypes.IgnoreQueryFilters().AnyAsync(x =>
            !x.IsDeleted && x.ReportTypeDefId == item.Id, ct))
            throw new ReportTypeConflictException("Bu rapor türü sağlık tesisine bağlı olduğundan silinemez.");
        if (await db.ReasonForExamReportTypes.IgnoreQueryFilters().AnyAsync(x =>
            !x.IsDeleted && x.ReportTypeDefId == item.Id, ct))
            throw new ReportTypeConflictException("Silinmemiş muayene sebebi eşleştirmesi bulunan tanım silinemez.");
        db.ReportTypeDefs.Remove(item);
        await db.SaveChangesAsync(ct);
    }
}
