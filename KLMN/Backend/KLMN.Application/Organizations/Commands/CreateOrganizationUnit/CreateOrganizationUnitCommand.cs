using MediatR;
using KLMN.Application.Organizations.Responses;

namespace KLMN.Application.Organizations.Commands.CreateOrganizationUnit;

/// <summary>CreateOrganizationUnit işleminin bağımsız CQRS komut sözleşmesidir.</summary>
public sealed record CreateOrganizationUnitCommand(string Name, string Code, Guid? ParentOrganizationUnitId) : IRequest<OrganizationUnitMutationResponse>;
