using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.SYYClassifications.Commands.Common;


using MediatR;
namespace KLMN.Application.SYYClassifications.Commands.DeleteSYYClassificationDef;
/// <summary>SYY Tanı Sınıflandırma Çizelgesi Delete handler; interceptor audit ve soft-delete uygular.</summary>
public sealed class DeleteSYYClassificationDefCommandHandler(IKLMNDbContext db):IRequestHandler<DeleteSYYClassificationDefCommand>
{
 public async Task Handle(DeleteSYYClassificationDefCommand r,CancellationToken ct)
 {
 var item=await SYYClassificationDefRules.LoadAsync(db,r.Id,r.Version,ct);
 db.SYYClassificationDefs.Remove(item);await db.SaveChangesAsync(ct);
 }
}
