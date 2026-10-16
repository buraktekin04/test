using KLMN.Application.MilitaryOffices.Responses;
using MediatR;

namespace KLMN.Application.MilitaryOffices.Commands.CreateMilitaryOfficeDef;

/// <summary>Askerlik şubesi Create CQRS isteğidir; xmin işlemlerde korunur.</summary>
public sealed record CreateMilitaryOfficeDefCommand(string Code, string Name, string? ExternalCode, Guid? CityId, DateTime? ValidFrom, DateTime? ValidTo) : IRequest<MilitaryOfficeDefMutationResponse>;
