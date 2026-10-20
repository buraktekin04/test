using KLMN.Application.Hospitals.Responses;
using MediatR;

namespace KLMN.Application.Hospitals.Queries.GetHospitalDefById;

/// <summary>xmin içeren sağlık tesisi detay sorgusu.</summary>
public sealed record GetHospitalDefByIdQuery(Guid Id) : IRequest<HospitalDefResponse>;
