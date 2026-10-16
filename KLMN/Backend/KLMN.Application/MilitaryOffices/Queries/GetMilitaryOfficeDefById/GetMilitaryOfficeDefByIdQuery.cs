using KLMN.Application.MilitaryOffices.Responses;
using MediatR;

namespace KLMN.Application.MilitaryOffices.Queries.GetMilitaryOfficeDefById;

/// <summary>Şubeyi güncel xmin sürümü ile detay olarak getirir.</summary>
public sealed record GetMilitaryOfficeDefByIdQuery(Guid Id) : IRequest<MilitaryOfficeDefResponse>;
