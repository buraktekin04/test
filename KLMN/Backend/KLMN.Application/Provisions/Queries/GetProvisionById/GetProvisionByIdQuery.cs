using KLMN.Application.Provisions.Responses;
using MediatR;

namespace KLMN.Application.Provisions.Queries.GetProvisionById;

public sealed record GetProvisionByIdQuery(Guid Id):IRequest<ProvisionDetailResponse>;
