using KLMN.Application.Common.Interfaces.Persistence;
 using KLMN.Application.PlaneTypes.Commands.Common;
 using KLMN.Application.PlaneTypes.Responses;
 
 using MediatR;
 namespace KLMN.Application.PlaneTypes.Commands.ChangeStatusPlaneTypeDef;
 /// <summary>Uçuşa Uçak Tipi Tanımları ChangeStatus iş akışı; BaseEntity audit/soft-delete/xmin kullanılır.</summary>
 public sealed class ChangeStatusPlaneTypeDefCommandHandler(IKLMNDbContext db):
 IRequestHandler<ChangeStatusPlaneTypeDefCommand,PlaneTypeDefMutationResponse>
 {
 public async Task<PlaneTypeDefMutationResponse> Handle(ChangeStatusPlaneTypeDefCommand r,CancellationToken ct){
 var item=await PlaneTypeDefRules.LoadAsync(db,r.Id,r.Version,ct);
 
 item.IsActive=r.IsActive;await db.SaveChangesAsync(ct);return new(item.Id,item.Version);
 }
 }
