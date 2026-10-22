using KLMN.Domain.Entities.Admissions;
using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.SYYDiagnosisRequireds.Commands.Common;
using KLMN.Application.SYYDiagnosisRequireds.Responses;

using MediatR;
namespace KLMN.Application.SYYDiagnosisRequireds.Commands.UpdateSYYDiagnosisRequired;
/// <summary>SYY Onay Gerektiren Maddeler — Update ve tek SaveChanges.</summary>
public sealed class UpdateSYYDiagnosisRequiredCommandHandler(IKLMNDbContext db):
 IRequestHandler<UpdateSYYDiagnosisRequiredCommand,SYYDiagnosisRequiredMutationResponse>
{
 public async Task<SYYDiagnosisRequiredMutationResponse> Handle(UpdateSYYDiagnosisRequiredCommand r,CancellationToken ct)
 {
 
 var item=await SYYDiagnosisRequiredRules.LoadAsync(db,r.Id,r.Version,ct);
 await SYYDiagnosisRequiredRules.ValidateDiagnosisAsync(db,r.SYYDiagnosisDefId,item.Id,ct);
 item.UpdatedDate=DateTime.UtcNow; // Detay değişikliğinde de parent xmin kontrolü.
 item.SYYDiagnosisDefId=r.SYYDiagnosisDefId;
 item.MustAll=r.MustAll;item.MustOneOf=r.MustOneOf;
 // Sıralı madde satırlarını mümkünse yerinde günceller; kaldırılanlar soft-delete olur.
 var present=item.Details.Where(x=>!x.IsDeleted).OrderBy(x=>x.SortOrder).ToList();
 for(var i=0;i<r.Descriptions.Count;i++)
 {
  if(i<present.Count){present[i].Description=r.Descriptions[i].Trim();present[i].SortOrder=i+1;}
  else item.Details.Add(new SYYDiagnosisRequiredDetail{Description=r.Descriptions[i].Trim(),SortOrder=i+1});
 }
 foreach(var old in present.Skip(r.Descriptions.Count))db.SYYDiagnosisRequiredDetails.Remove(old);
 await db.SaveChangesAsync(ct);return new(item.Id,item.Version);
 }
}
