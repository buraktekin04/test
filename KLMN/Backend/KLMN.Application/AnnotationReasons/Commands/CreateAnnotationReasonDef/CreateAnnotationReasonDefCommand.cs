using MediatR;
 using KLMN.Application.AnnotationReasons.Responses;
 namespace KLMN.Application.AnnotationReasons.Commands.CreateAnnotationReasonDef;
 /// <summary>Zeyil Sebebi Tanımları: Create komutu.</summary>
 public sealed record CreateAnnotationReasonDefCommand(string Code,string Name,DateTime? ValidFrom,DateTime? ValidTo):IRequest<AnnotationReasonDefMutationResponse>;
