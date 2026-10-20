using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.HospitalReportTypes.Commands.Common;
using MediatR;

namespace KLMN.Application.HospitalReportTypes.Commands.DeleteHospitalReportType;

/// <summary>Sağlık Tesisi Rapor Türü Delete iş akışını EF ve audit altyapısında yürütür.</summary>
public sealed class DeleteHospitalReportTypeCommandHandler(IKLMNDbContext db)
    : IRequestHandler<DeleteHospitalReportTypeCommand>
{
    /// <summary>Validasyonu yapılan isteği concurrency korumasıyla kaydeder.</summary>
    public async Task Handle(DeleteHospitalReportTypeCommand request, CancellationToken ct)
    {
        var item = await HospitalReportTypeRules.LoadForChangeAsync(db, request.Id, request.Version, ct);
        db.HospitalReportTypes.Remove(item);
        await db.SaveChangesAsync(ct);
    }
}
