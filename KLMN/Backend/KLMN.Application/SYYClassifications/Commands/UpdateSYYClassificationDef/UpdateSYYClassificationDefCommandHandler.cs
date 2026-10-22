using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.SYYClassifications.Commands.Common;
using KLMN.Application.SYYClassifications.Responses;

using MediatR;
namespace KLMN.Application.SYYClassifications.Commands.UpdateSYYClassificationDef;
/// <summary>SYY Tanı Sınıflandırma Çizelgesi Update handler; interceptor audit ve soft-delete uygular.</summary>
public sealed class UpdateSYYClassificationDefCommandHandler(IKLMNDbContext db):IRequestHandler<UpdateSYYClassificationDefCommand,SYYClassificationDefMutationResponse>
{
 public async Task<SYYClassificationDefMutationResponse> Handle(UpdateSYYClassificationDefCommand r,CancellationToken ct)
 {
 
 var item=await SYYClassificationDefRules.LoadAsync(db,r.Id,r.Version,ct);
 var fields=new SYYClassificationDefFields(r.SYYDiagnosisDefId,r.ForcesCommandDef,r.RankGroup,r.MilitaryClassGroup,r.HcDecision,r.ValidFrom,r.ValidTo);
 await SYYClassificationDefRules.EnsureAsync(db,fields,item.Id,ct,item);
 item.SYYDiagnosisDefId=r.SYYDiagnosisDefId;
item.ForcesCommandDef=r.ForcesCommandDef;
item.RankGroup=r.RankGroup;
item.MilitaryClassGroup=r.MilitaryClassGroup;
item.HcDecision=r.HcDecision;
item.ValidFrom=r.ValidFrom;
item.ValidTo=r.ValidTo;
 await db.SaveChangesAsync(ct);return new(item.Id,item.Version);
 }
}
