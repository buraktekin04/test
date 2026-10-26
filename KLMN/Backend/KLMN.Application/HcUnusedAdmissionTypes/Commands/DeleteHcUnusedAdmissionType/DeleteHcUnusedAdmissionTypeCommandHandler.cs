using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.HcUnusedAdmissionTypes.Commands.Common;


using MediatR;
namespace KLMN.Application.HcUnusedAdmissionTypes.Commands.DeleteHcUnusedAdmissionType;
/// <summary>Kullanılmayacak Başvuru Grupları için Delete işini veri bütünlüğüyle yürütür.</summary>
public sealed class DeleteHcUnusedAdmissionTypeCommandHandler(IKLMNDbContext db):IRequestHandler<DeleteHcUnusedAdmissionTypeCommand>
{
 public async Task Handle(DeleteHcUnusedAdmissionTypeCommand r,CancellationToken ct)
 {
    var item=await HcUnusedAdmissionTypeRules.LoadAsync(db,r.Id,r.Version,ct);
    db.HcUnusedAdmissionTypes.Remove(item);await db.SaveChangesAsync(ct);
 }
}
