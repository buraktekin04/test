using MediatR;
using KLMN.Application.Organizations.Responses;

namespace KLMN.Application.Organizations.Commands.ChangeOrganizationUnitStatus;

/// <summary>ChangeOrganizationUnitStatus işleminin bağımsız CQRS komut sözleşmesidir.</summary>
public sealed record ChangeOrganizationUnitStatusCommand(Guid Id, uint Version, bool IsActive) : IRequest<OrganizationUnitMutationResponse>;
