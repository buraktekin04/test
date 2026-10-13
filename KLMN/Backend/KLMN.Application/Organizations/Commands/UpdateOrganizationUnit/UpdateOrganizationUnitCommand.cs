using MediatR;
using KLMN.Application.Organizations.Responses;

namespace KLMN.Application.Organizations.Commands.UpdateOrganizationUnit;

/// <summary>UpdateOrganizationUnit işleminin bağımsız CQRS komut sözleşmesidir.</summary>
public sealed record UpdateOrganizationUnitCommand(Guid Id, uint Version, string Name, string Code, Guid? ParentOrganizationUnitId) : IRequest<OrganizationUnitMutationResponse>;
