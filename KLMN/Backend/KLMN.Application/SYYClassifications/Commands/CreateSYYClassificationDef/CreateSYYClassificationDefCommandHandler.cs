using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.SYYClassifications.Commands.Common;
using KLMN.Application.SYYClassifications.Responses;
using KLMN.Domain.Entities.Admissions;
using MediatR;
namespace KLMN.Application.SYYClassifications.Commands.CreateSYYClassificationDef;
/// <summary>SYY Tanı Sınıflandırma Çizelgesi Create handler; interceptor audit ve soft-delete uygular.</summary>
public sealed class CreateSYYClassificationDefCommandHandler(IKLMNDbContext db):IRequestHandler<CreateSYYClassificationDefCommand,SYYClassificationDefMutationResponse>
{
 public async Task<SYYClassificationDefMutationResponse> Handle(CreateSYYClassificationDefCommand r,CancellationToken ct)
 {
 
 var fields=new SYYClassificationDefFields(r.SYYDiagnosisDefId,r.ForcesCommandDef,r.RankGroup,r.MilitaryClassGroup,r.HcDecision,r.ValidFrom,r.ValidTo);
 await SYYClassificationDefRules.EnsureAsync(db,fields,null,ct);
 var item=new SYYClassificationDef{SYYDiagnosisDefId=r.SYYDiagnosisDefId,ForcesCommandDef=r.ForcesCommandDef,RankGroup=r.RankGroup,MilitaryClassGroup=r.MilitaryClassGroup,HcDecision=r.HcDecision,ValidFrom=r.ValidFrom,ValidTo=r.ValidTo};
 await db.SYYClassificationDefs.AddAsync(item,ct);
 await db.SaveChangesAsync(ct);return new(item.Id,item.Version);
 }
}
