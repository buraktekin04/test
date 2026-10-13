using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Roles.Exceptions;
using KLMN.Application.Roles.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Roles.Queries.GetRoleById;

/// <summary>Düzenleme/yetkilendirme öncesi güncel rol verisini ve xmin bilgisini getirir.</summary>
public sealed record GetRoleByIdQuery(Guid Id) : IRequest<RoleResponse>;
