using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.SYYDiagnosisRequireds.Commands.Common;


using MediatR;
namespace KLMN.Application.SYYDiagnosisRequireds.Commands.DeleteSYYDiagnosisRequired;
/// <summary>SYY Onay Gerektiren Maddeler — Delete ve tek SaveChanges.</summary>
public sealed class DeleteSYYDiagnosisRequiredCommandHandler(IKLMNDbContext db):
 IRequestHandler<DeleteSYYDiagnosisRequiredCommand>
{
 public async Task Handle(DeleteSYYDiagnosisRequiredCommand r,CancellationToken ct)
 {
 
 var item=await SYYDiagnosisRequiredRules.LoadAsync(db,r.Id,r.Version,ct);
 foreach(var d in item.Details.Where(x=>!x.IsDeleted))db.SYYDiagnosisRequiredDetails.Remove(d);
 db.SYYDiagnosisRequireds.Remove(item);await db.SaveChangesAsync(ct);
 }
}
