using KLMN.Application.AnnotationReasons.Responses;
using MediatR;
namespace KLMN.Application.AnnotationReasons.Queries.GetAnnotationReasonDefById;
/// <summary>Sevk Sebebi Tanımları tek kayıt sorgusu.</summary>
public sealed record GetAnnotationReasonDefByIdQuery(Guid Id):IRequest<AnnotationReasonDefResponse>;
