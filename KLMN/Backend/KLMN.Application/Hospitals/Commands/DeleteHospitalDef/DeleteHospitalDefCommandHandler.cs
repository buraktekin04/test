using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Hospitals.Commands.Common;
using MediatR;

namespace KLMN.Application.Hospitals.Commands.DeleteHospitalDef;

/// <summary>Sağlık Tesisi Delete iş akışını EF ve audit altyapısında yürütür.</summary>
public sealed class DeleteHospitalDefCommandHandler(IKLMNDbContext db)
    : IRequestHandler<DeleteHospitalDefCommand>
{
    /// <summary>Validasyonu yapılan isteği concurrency korumasıyla kaydeder.</summary>
    public async Task Handle(DeleteHospitalDefCommand request, CancellationToken ct)
    {
        var item = await HospitalDefRules.LoadForChangeAsync(db, request.Id, request.Version, ct);
        await HospitalDefRules.EnsureNoDependentsAsync(db, item.Id, false, ct);
        db.HospitalDefs.Remove(item);
        await db.SaveChangesAsync(ct);
    }
}
