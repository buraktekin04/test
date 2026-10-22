using MediatR;
 
 namespace KLMN.Application.AnnotationReasons.Commands.DeleteAnnotationReasonDef;
 /// <summary>Sevk Sebebi Tanımları: Delete komutu.</summary>
 public sealed record DeleteAnnotationReasonDefCommand(Guid Id,uint Version):IRequest;
