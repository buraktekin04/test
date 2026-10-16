using KLMN.Application.AdmissionTypes.Responses;
using MediatR;

namespace KLMN.Application.AdmissionTypes.Queries.GetHcAdmissionTypeDefById;

/// <summary>Düzenlemeye ait güncel xmin ve zorunluluk bayraklarını getirir.</summary>
public sealed record GetHcAdmissionTypeDefByIdQuery(Guid Id) : IRequest<HcAdmissionTypeDefResponse>;
