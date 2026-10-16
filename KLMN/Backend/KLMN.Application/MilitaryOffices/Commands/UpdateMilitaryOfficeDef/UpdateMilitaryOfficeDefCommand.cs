using KLMN.Application.MilitaryOffices.Responses;
using MediatR;

namespace KLMN.Application.MilitaryOffices.Commands.UpdateMilitaryOfficeDef;

/// <summary>Askerlik şubesi Update CQRS isteğidir; xmin işlemlerde korunur.</summary>
public sealed record UpdateMilitaryOfficeDefCommand(Guid Id, uint Version, string Code, string Name, string? ExternalCode, Guid? CityId, DateTime? ValidFrom, DateTime? ValidTo) : IRequest<MilitaryOfficeDefMutationResponse>;
