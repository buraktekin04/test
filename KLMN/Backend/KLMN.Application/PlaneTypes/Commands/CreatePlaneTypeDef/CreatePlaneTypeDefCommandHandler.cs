using KLMN.Application.Common.Interfaces.Persistence;
 using KLMN.Application.PlaneTypes.Commands.Common;
 using KLMN.Application.PlaneTypes.Responses;
 using KLMN.Domain.Entities.Admissions;
 using MediatR;
 namespace KLMN.Application.PlaneTypes.Commands.CreatePlaneTypeDef;
 /// <summary>Uçuşa Uçak Tipi Tanımları Create iş akışı; BaseEntity audit/soft-delete/xmin kullanılır.</summary>
 public sealed class CreatePlaneTypeDefCommandHandler(IKLMNDbContext db):
 IRequestHandler<CreatePlaneTypeDefCommand,PlaneTypeDefMutationResponse>
 {
 public async Task<PlaneTypeDefMutationResponse> Handle(CreatePlaneTypeDefCommand r,CancellationToken ct){
 await PlaneTypeDefRules.EnsureUniqueAsync(db,r.Code,null,ct);
 
 var item=new PlaneTypeDef{Code=r.Code.Trim().ToUpperInvariant(),Name=r.Name.Trim(),ValidFrom=r.ValidFrom,ValidTo=r.ValidTo};
 await db.PlaneTypeDefs.AddAsync(item,ct);await db.SaveChangesAsync(ct);return new(item.Id,item.Version);
 }
 }
