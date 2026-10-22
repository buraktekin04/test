using MediatR;
 using KLMN.Application.PlaneTypes.Responses;
 namespace KLMN.Application.PlaneTypes.Commands.CreatePlaneTypeDef;
 /// <summary>Uçuşa Uçak Tipi Tanımları: Create komutu.</summary>
 public sealed record CreatePlaneTypeDefCommand(string Code,string Name,DateTime? ValidFrom,DateTime? ValidTo):IRequest<PlaneTypeDefMutationResponse>;
