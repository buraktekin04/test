using MediatR;
 using KLMN.Application.AnnotationReasons.Responses;
 namespace KLMN.Application.AnnotationReasons.Commands.UpdateAnnotationReasonDef;
 /// <summary>Zeyil Sebebi Tanımları: Update komutu.</summary>
 public sealed record UpdateAnnotationReasonDefCommand(Guid Id,uint Version,string Code,string Name,DateTime? ValidFrom,DateTime? ValidTo):IRequest<AnnotationReasonDefMutationResponse>;
