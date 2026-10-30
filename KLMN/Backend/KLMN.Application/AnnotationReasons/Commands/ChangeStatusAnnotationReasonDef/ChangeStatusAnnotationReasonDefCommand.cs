using MediatR;
 using KLMN.Application.AnnotationReasons.Responses;
 namespace KLMN.Application.AnnotationReasons.Commands.ChangeStatusAnnotationReasonDef;
 /// <summary>Zeyil Sebebi Tanımları: ChangeStatus komutu.</summary>
 public sealed record ChangeStatusAnnotationReasonDefCommand(Guid Id,uint Version,bool IsActive):IRequest<AnnotationReasonDefMutationResponse>;
