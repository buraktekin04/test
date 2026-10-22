using MediatR;
 using KLMN.Application.PlaneTypes.Responses;
 namespace KLMN.Application.PlaneTypes.Commands.UpdatePlaneTypeDef;
 /// <summary>Uçuşa Uçak Tipi Tanımları: Update komutu.</summary>
 public sealed record UpdatePlaneTypeDefCommand(Guid Id,uint Version,string Code,string Name,DateTime? ValidFrom,DateTime? ValidTo):IRequest<PlaneTypeDefMutationResponse>;
