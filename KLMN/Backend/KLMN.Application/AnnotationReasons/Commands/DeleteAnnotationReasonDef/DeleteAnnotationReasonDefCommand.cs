using MediatR;
 
 namespace KLMN.Application.AnnotationReasons.Commands.DeleteAnnotationReasonDef;
 /// <summary>Zeyil Sebebi Tanımları: Delete komutu.</summary>
 public sealed record DeleteAnnotationReasonDefCommand(Guid Id,uint Version):IRequest;
