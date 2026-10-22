using KLMN.Application.Common.Interfaces.Persistence;
 using KLMN.Application.FlyingMissions.Commands.Common;
 using KLMN.Application.FlyingMissions.Responses;
 using KLMN.Domain.Entities.Admissions;
 using MediatR;
 namespace KLMN.Application.FlyingMissions.Commands.CreateFlyingMissionDef;
 /// <summary>Uçucu Görev Tanımları Create iş akışı; BaseEntity audit/soft-delete/xmin kullanılır.</summary>
 public sealed class CreateFlyingMissionDefCommandHandler(IKLMNDbContext db):
 IRequestHandler<CreateFlyingMissionDefCommand,FlyingMissionDefMutationResponse>
 {
 public async Task<FlyingMissionDefMutationResponse> Handle(CreateFlyingMissionDefCommand r,CancellationToken ct){
 await FlyingMissionDefRules.EnsureUniqueAsync(db,r.Code,null,ct);
 
 var item=new FlyingMissionDef{Code=r.Code.Trim().ToUpperInvariant(),Name=r.Name.Trim(),ValidFrom=r.ValidFrom,ValidTo=r.ValidTo};
 await db.FlyingMissionDefs.AddAsync(item,ct);await db.SaveChangesAsync(ct);return new(item.Id,item.Version);
 }
 }
