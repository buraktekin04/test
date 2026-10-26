using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.HcUnusedAdmissionTypes.Commands.Common;
using KLMN.Domain.Entities.Admissions;
using KLMN.Application.HcUnusedAdmissionTypes.Responses;
using MediatR;
namespace KLMN.Application.HcUnusedAdmissionTypes.Commands.CreateHcUnusedAdmissionType;
/// <summary>Kullanılmayacak Başvuru Grupları için Create işini veri bütünlüğüyle yürütür.</summary>
public sealed class CreateHcUnusedAdmissionTypeCommandHandler(IKLMNDbContext db):IRequestHandler<CreateHcUnusedAdmissionTypeCommand,HcUnusedAdmissionTypeMutationResponse>
{
 public async Task<HcUnusedAdmissionTypeMutationResponse> Handle(CreateHcUnusedAdmissionTypeCommand r,CancellationToken ct)
 {
    await HcUnusedAdmissionTypeRules.EnsureReferencesAsync(db,r.HcDecisionDefId,r.ReferenceId,ct);
    await HcUnusedAdmissionTypeRules.EnsureUniqueAsync(db,r.HcDecisionDefId,r.ReferenceId,null,ct);
    var item=new HcUnusedAdmissionType{HcDecisionDefId=r.HcDecisionDefId,HcAdmissionTypeDefId=r.ReferenceId};
    await db.HcUnusedAdmissionTypes.AddAsync(item,ct);await db.SaveChangesAsync(ct);
    return new(item.Id,item.Version);
 }
}
