using MediatR;
 using KLMN.Application.PlaneTypes.Responses;
 namespace KLMN.Application.PlaneTypes.Commands.ChangeStatusPlaneTypeDef;
 /// <summary>Uçuşa Uçak Tipi Tanımları: ChangeStatus komutu.</summary>
 public sealed record ChangeStatusPlaneTypeDefCommand(Guid Id,uint Version,bool IsActive):IRequest<PlaneTypeDefMutationResponse>;
