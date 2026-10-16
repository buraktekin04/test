using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.AdmissionTypes.Commands.Common;
using KLMN.Application.AdmissionTypes.Responses;
using MediatR;

namespace KLMN.Application.AdmissionTypes.Commands.ChangeStatusHcAdmissionTypeDef;

/// <summary>İşlemi güncel xmin ve audit/soft-delete interceptor'ı üzerinden yürütür.</summary>
public sealed class ChangeStatusHcAdmissionTypeDefCommandHandler(IKLMNDbContext db)
    : IRequestHandler<ChangeStatusHcAdmissionTypeDefCommand, HcAdmissionTypeDefMutationResponse>
{
    /// <summary>İlgili işlem için geçerli kayıt sürümünü kontrol eder.</summary>
    public async Task<HcAdmissionTypeDefMutationResponse> Handle(
        ChangeStatusHcAdmissionTypeDefCommand request, CancellationToken ct)
    {
        var item = await HcAdmissionTypeDefRules.LoadForChangeAsync(db, request.Id, request.Version, ct);
        item.IsActive = request.IsActive;
        await db.SaveChangesAsync(ct);
        return new HcAdmissionTypeDefMutationResponse(item.Id, item.Version);
    }
}
