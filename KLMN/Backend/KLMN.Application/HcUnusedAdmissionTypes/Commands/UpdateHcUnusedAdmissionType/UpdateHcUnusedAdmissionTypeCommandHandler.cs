using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.HcUnusedAdmissionTypes.Commands.Common;

using KLMN.Application.HcUnusedAdmissionTypes.Responses;
using MediatR;
namespace KLMN.Application.HcUnusedAdmissionTypes.Commands.UpdateHcUnusedAdmissionType;
/// <summary>Kullanılmayacak Başvuru Grupları için Update işini veri bütünlüğüyle yürütür.</summary>
public sealed class UpdateHcUnusedAdmissionTypeCommandHandler(IKLMNDbContext db):IRequestHandler<UpdateHcUnusedAdmissionTypeCommand,HcUnusedAdmissionTypeMutationResponse>
{
 public async Task<HcUnusedAdmissionTypeMutationResponse> Handle(UpdateHcUnusedAdmissionTypeCommand r,CancellationToken ct)
 {
    var item=await HcUnusedAdmissionTypeRules.LoadAsync(db,r.Id,r.Version,ct);
    await HcUnusedAdmissionTypeRules.EnsureReferencesAsync(db,r.HcDecisionDefId,r.ReferenceId,ct,item.HcDecisionDefId,item.HcAdmissionTypeDefId);
    await HcUnusedAdmissionTypeRules.EnsureUniqueAsync(db,r.HcDecisionDefId,r.ReferenceId,item.Id,ct);
    item.HcDecisionDefId=r.HcDecisionDefId;item.HcAdmissionTypeDefId=r.ReferenceId;
    await db.SaveChangesAsync(ct);return new(item.Id,item.Version);
 }
}
