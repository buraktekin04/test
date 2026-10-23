using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.ICD10Defs.Commands.Common;
using KLMN.Application.ICD10Defs.Responses;
using KLMN.Domain.Entities.Admissions;
using MediatR;
namespace KLMN.Application.ICD10Defs.Commands.CreateICD10Def;
/// <summary>ICD10 Tanımları — Create işlemi, soft-delete/audit/xmin mevcut altyapıda.</summary>
public sealed class CreateICD10DefCommandHandler(IKLMNDbContext db):IRequestHandler<CreateICD10DefCommand,ICD10DefMutationResponse>
{
 public async Task<ICD10DefMutationResponse> Handle(CreateICD10DefCommand r,CancellationToken ct)
 {
 var code=r.Code.Trim().ToUpperInvariant();
 await ICD10DefRules.EnsureUniqueAsync(db,code,null,ct);
 var item=new ICD10Def{Code=code,Name=r.Name.Trim()};
 await db.ICD10Defs.AddAsync(item,ct);
 await db.SaveChangesAsync(ct);return new(item.Id,item.Version);
 }
}
