using MediatR;
 
 namespace KLMN.Application.PlaneTypes.Commands.DeletePlaneTypeDef;
 /// <summary>Uçuşa Uçak Tipi Tanımları: Delete komutu.</summary>
 public sealed record DeletePlaneTypeDefCommand(Guid Id,uint Version):IRequest;
