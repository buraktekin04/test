using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.HospitalReportTypes.Commands.Common;
using KLMN.Application.HospitalReportTypes.Responses;
using KLMN.Domain.Entities.Admissions;
using MediatR;

namespace KLMN.Application.HospitalReportTypes.Commands.CreateHospitalReportType;

/// <summary>Sağlık Tesisi Rapor Türü Create iş akışını EF ve audit altyapısında yürütür.</summary>
public sealed class CreateHospitalReportTypeCommandHandler(IKLMNDbContext db)
    : IRequestHandler<CreateHospitalReportTypeCommand, HospitalReportTypeMutationResponse>
{
    /// <summary>Validasyonu yapılan isteği concurrency korumasıyla kaydeder.</summary>
    public async Task<HospitalReportTypeMutationResponse> Handle(CreateHospitalReportTypeCommand request, CancellationToken ct)
    {
        await HospitalReportTypeRules.EnsureUniquePairAsync(db, request.HospitalDefId,
            request.ReportTypeDefId, null, ct);
        await HospitalReportTypeRules.EnsureReferencesAsync(db, request.HospitalDefId,
            request.ReportTypeDefId, ct);
        var item = new HospitalReportType
        {
            HospitalDefId = request.HospitalDefId,
            ReportTypeDefId = request.ReportTypeDefId
        };
        await db.HospitalReportTypes.AddAsync(item, ct);
        await db.SaveChangesAsync(ct);
        return new HospitalReportTypeMutationResponse(item.Id, item.Version);
    }
}
