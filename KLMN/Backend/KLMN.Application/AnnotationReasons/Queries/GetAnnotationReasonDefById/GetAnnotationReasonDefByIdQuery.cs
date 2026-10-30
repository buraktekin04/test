using KLMN.Application.AnnotationReasons.Responses;
using MediatR;
namespace KLMN.Application.AnnotationReasons.Queries.GetAnnotationReasonDefById;
/// <summary>Zeyil Sebebi Tanımları tek kayıt sorgusu.</summary>
public sealed record GetAnnotationReasonDefByIdQuery(Guid Id):IRequest<AnnotationReasonDefResponse>;
