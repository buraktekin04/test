using MediatR;

namespace KLMN.Application.Organizations.Commands.DeleteOrganizationUnit;

/// <summary>DeleteOrganizationUnit işleminin bağımsız CQRS komut sözleşmesidir.</summary>
public sealed record DeleteOrganizationUnitCommand(Guid Id, uint Version) : IRequest;
