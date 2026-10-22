using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.SYYClassifications.Commands.Common;
using KLMN.Application.SYYClassifications.Responses;

using MediatR;
namespace KLMN.Application.SYYClassifications.Commands.ChangeStatusSYYClassificationDef;
/// <summary>SYY Tanı Sınıflandırma Çizelgesi ChangeStatus handler; interceptor audit ve soft-delete uygular.</summary>
public sealed class ChangeStatusSYYClassificationDefCommandHandler(IKLMNDbContext db):IRequestHandler<ChangeStatusSYYClassificationDefCommand,SYYClassificationDefMutationResponse>
{
 public async Task<SYYClassificationDefMutationResponse> Handle(ChangeStatusSYYClassificationDefCommand r,CancellationToken ct)
 {
 
 var item=await SYYClassificationDefRules.LoadAsync(db,r.Id,r.Version,ct);
 item.IsActive=r.IsActive;await db.SaveChangesAsync(ct);return new(item.Id,item.Version);
 }
}
