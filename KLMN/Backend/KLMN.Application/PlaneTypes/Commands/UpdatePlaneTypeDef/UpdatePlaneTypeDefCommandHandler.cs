using KLMN.Application.Common.Interfaces.Persistence;
 using KLMN.Application.PlaneTypes.Commands.Common;
 using KLMN.Application.PlaneTypes.Responses;
 
 using MediatR;
 namespace KLMN.Application.PlaneTypes.Commands.UpdatePlaneTypeDef;
 /// <summary>Uçuşa Uçak Tipi Tanımları Update iş akışı; BaseEntity audit/soft-delete/xmin kullanılır.</summary>
 public sealed class UpdatePlaneTypeDefCommandHandler(IKLMNDbContext db):
 IRequestHandler<UpdatePlaneTypeDefCommand,PlaneTypeDefMutationResponse>
 {
 public async Task<PlaneTypeDefMutationResponse> Handle(UpdatePlaneTypeDefCommand r,CancellationToken ct){
 var item=await PlaneTypeDefRules.LoadAsync(db,r.Id,r.Version,ct);
 await PlaneTypeDefRules.EnsureUniqueAsync(db,r.Code,item.Id,ct);
 
 item.Code=r.Code.Trim().ToUpperInvariant();item.Name=r.Name.Trim();item.ValidFrom=r.ValidFrom;item.ValidTo=r.ValidTo;
 await db.SaveChangesAsync(ct);return new(item.Id,item.Version);
 }
 }
