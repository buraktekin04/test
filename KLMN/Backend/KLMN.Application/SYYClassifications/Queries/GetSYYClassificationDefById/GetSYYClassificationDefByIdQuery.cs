using KLMN.Application.SYYClassifications.Responses;
using MediatR;
namespace KLMN.Application.SYYClassifications.Queries.GetSYYClassificationDefById;
public sealed record GetSYYClassificationDefByIdQuery(Guid Id):IRequest<SYYClassificationDefResponse>;
