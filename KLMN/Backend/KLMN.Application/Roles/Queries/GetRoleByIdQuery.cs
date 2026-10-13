using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Roles.Exceptions;
using KLMN.Application.Roles.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Roles.Queries;

/// <summary>Düzenleme/yetkilendirme öncesi güncel rol verisini ve xmin bilgisini getirir.</summary>
public sealed record GetRoleByIdQuery(Guid Id) : IRequest<RoleResponse>;

/// <summary>Pasif roller dahil, soft-delete edilmemiş rol detayını döndürür.</summary>
public sealed class GetRoleByIdQueryHandler(IKLMNDbContext dbContext)
    : IRequestHandler<GetRoleByIdQuery, RoleResponse>
{
    /// <summary>Kimlik ile projeksiyon yapar; bulunamazsa 404 oluşturur.</summary>
    public async Task<RoleResponse> Handle(
        GetRoleByIdQuery request, CancellationToken cancellationToken) =>
        await dbContext.Roles.IgnoreQueryFilters()
            .AsNoTracking()
            .Where(x => x.Id == request.Id && !x.IsDeleted)
            .Select(RoleResponse.Projection)
            .SingleOrDefaultAsync(cancellationToken)
        ?? throw new RoleNotFoundException();
}
