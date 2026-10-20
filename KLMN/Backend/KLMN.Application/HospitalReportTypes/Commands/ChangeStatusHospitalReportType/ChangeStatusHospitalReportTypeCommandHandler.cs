using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.HospitalReportTypes.Commands.Common;
using KLMN.Application.HospitalReportTypes.Responses;
using MediatR;

namespace KLMN.Application.HospitalReportTypes.Commands.ChangeStatusHospitalReportType;

/// <summary>Sağlık Tesisi Rapor Türü ChangeStatus iş akışını EF ve audit altyapısında yürütür.</summary>
public sealed class ChangeStatusHospitalReportTypeCommandHandler(IKLMNDbContext db)
    : IRequestHandler<ChangeStatusHospitalReportTypeCommand, HospitalReportTypeMutationResponse>
{
    /// <summary>Validasyonu yapılan isteği concurrency korumasıyla kaydeder.</summary>
    public async Task<HospitalReportTypeMutationResponse> Handle(ChangeStatusHospitalReportTypeCommand request, CancellationToken ct)
    {
        var item = await HospitalReportTypeRules.LoadForChangeAsync(db, request.Id, request.Version, ct);
        if (request.IsActive)
            await HospitalReportTypeRules.EnsureReferencesAsync(db, item.HospitalDefId,
                item.ReportTypeDefId, ct);
        item.IsActive = request.IsActive;
        await db.SaveChangesAsync(ct);
        return new HospitalReportTypeMutationResponse(item.Id, item.Version);
    }
}
