using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.SYYDiagnosisRequireds.Commands.Common;
using KLMN.Application.SYYDiagnosisRequireds.Responses;
using KLMN.Domain.Entities.Admissions;
using MediatR;
namespace KLMN.Application.SYYDiagnosisRequireds.Commands.CreateSYYDiagnosisRequired;
/// <summary>SYY Onay Gerektiren Maddeler — Create ve tek SaveChanges.</summary>
public sealed class CreateSYYDiagnosisRequiredCommandHandler(IKLMNDbContext db):
 IRequestHandler<CreateSYYDiagnosisRequiredCommand,SYYDiagnosisRequiredMutationResponse>
{
 public async Task<SYYDiagnosisRequiredMutationResponse> Handle(CreateSYYDiagnosisRequiredCommand r,CancellationToken ct)
 {
 
 await SYYDiagnosisRequiredRules.ValidateDiagnosisAsync(db,r.SYYDiagnosisDefId,null,ct);
 var item=new SYYDiagnosisRequired{SYYDiagnosisDefId=r.SYYDiagnosisDefId,MustAll=r.MustAll,MustOneOf=r.MustOneOf};
 for(var i=0;i<r.Descriptions.Count;i++)item.Details.Add(new SYYDiagnosisRequiredDetail
 {Description=r.Descriptions[i].Trim(),SortOrder=i+1});
 await db.SYYDiagnosisRequireds.AddAsync(item,ct);
 await db.SaveChangesAsync(ct);return new(item.Id,item.Version);
 }
}
