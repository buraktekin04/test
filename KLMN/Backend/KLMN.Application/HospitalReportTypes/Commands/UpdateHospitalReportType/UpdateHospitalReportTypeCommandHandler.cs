using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.HospitalReportTypes.Commands.Common;
using KLMN.Application.HospitalReportTypes.Responses;
using MediatR;

namespace KLMN.Application.HospitalReportTypes.Commands.UpdateHospitalReportType;

/// <summary>Sağlık Tesisi Rapor Türü Update iş akışını EF ve audit altyapısında yürütür.</summary>
public sealed class UpdateHospitalReportTypeCommandHandler(IKLMNDbContext db)
    : IRequestHandler<UpdateHospitalReportTypeCommand, HospitalReportTypeMutationResponse>
{
    /// <summary>Validasyonu yapılan isteği concurrency korumasıyla kaydeder.</summary>
    public async Task<HospitalReportTypeMutationResponse> Handle(UpdateHospitalReportTypeCommand request, CancellationToken ct)
    {
        var item = await HospitalReportTypeRules.LoadForChangeAsync(db, request.Id, request.Version, ct);
        await HospitalReportTypeRules.EnsureUniquePairAsync(db, request.HospitalDefId,
            request.ReportTypeDefId, item.Id, ct);
        await HospitalReportTypeRules.EnsureReferencesAsync(db, request.HospitalDefId,
            request.ReportTypeDefId, ct, item.HospitalDefId, item.ReportTypeDefId);
        item.HospitalDefId = request.HospitalDefId;
        item.ReportTypeDefId = request.ReportTypeDefId;
        await db.SaveChangesAsync(ct);
        return new HospitalReportTypeMutationResponse(item.Id, item.Version);
    }
}
