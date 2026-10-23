using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.ICD10Defs.Commands.Common;
using KLMN.Application.ICD10Defs.Responses;

using MediatR;
namespace KLMN.Application.ICD10Defs.Commands.UpdateICD10Def;
/// <summary>ICD10 Tanımları — Update işlemi, soft-delete/audit/xmin mevcut altyapıda.</summary>
public sealed class UpdateICD10DefCommandHandler(IKLMNDbContext db):IRequestHandler<UpdateICD10DefCommand,ICD10DefMutationResponse>
{
 public async Task<ICD10DefMutationResponse> Handle(UpdateICD10DefCommand r,CancellationToken ct)
 {
 var item=await ICD10DefRules.LoadAsync(db,r.Id,r.Version,ct);
 var code=r.Code.Trim().ToUpperInvariant();
 await ICD10DefRules.EnsureUniqueAsync(db,code,r.Id,ct);
 item.Code=code;item.Name=r.Name.Trim();
 await db.SaveChangesAsync(ct);return new(item.Id,item.Version);
 }
}
