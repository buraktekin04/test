using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.HcUnusedAdmissionTypes.Commands.Common;

using KLMN.Application.HcUnusedAdmissionTypes.Responses;
using MediatR;
namespace KLMN.Application.HcUnusedAdmissionTypes.Commands.ChangeStatusHcUnusedAdmissionType;
/// <summary>Kullanılmayacak Başvuru Grupları için ChangeStatus işini veri bütünlüğüyle yürütür.</summary>
public sealed class ChangeStatusHcUnusedAdmissionTypeCommandHandler(IKLMNDbContext db):IRequestHandler<ChangeStatusHcUnusedAdmissionTypeCommand,HcUnusedAdmissionTypeMutationResponse>
{
 public async Task<HcUnusedAdmissionTypeMutationResponse> Handle(ChangeStatusHcUnusedAdmissionTypeCommand r,CancellationToken ct)
 {
    var item=await HcUnusedAdmissionTypeRules.LoadAsync(db,r.Id,r.Version,ct);
    if(r.IsActive)await HcUnusedAdmissionTypeRules.EnsureReferencesAsync(db,item.HcDecisionDefId,item.HcAdmissionTypeDefId,ct);
    item.IsActive=r.IsActive;await db.SaveChangesAsync(ct);return new(item.Id,item.Version);
 }
}
