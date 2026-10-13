using KLMN.Application.Auditing.Responses;
using KLMN.Application.Auditing.Services;
using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Auditing.Queries.GetAuditLogs;

/// <summary>Silinmiş kayıtlar dahil tüm audit geçmişini bağımsız audit tablosundan sorgular.</summary>
public sealed class GetAuditLogsQueryHandler(IKLMNDbContext dbContext)
    : IRequestHandler<GetAuditLogsQuery, PagedResult<AuditLogResponse>>
{
    public async Task<PagedResult<AuditLogResponse>> Handle(
        GetAuditLogsQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.AuditLogs.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.EntityType) && request.EntityId.HasValue)
        {
            // İlişkisel izin/rol/PDF değişiklikleri de ana kullanıcı veya rol
            // satırının geçmişinde görünür; genel tabloda gerçek tür saklanır.
            query = query.Where(x =>
                (x.EntityType == request.EntityType && x.EntityId == request.EntityId.Value) ||
                (x.RelatedEntityType == request.EntityType && x.RelatedEntityId == request.EntityId.Value));
        }
        else if (!string.IsNullOrWhiteSpace(request.EntityType))
        {
            query = query.Where(x => x.EntityType == request.EntityType);
        }

        if (!string.IsNullOrWhiteSpace(request.Action))
            query = query.Where(x => x.Action == request.Action);

        if (request.ActorUserId.HasValue)
            query = query.Where(x => x.ActorUserId == request.ActorUserId);

        if (request.From.HasValue)
            query = query.Where(x => x.OccurredAt >= request.From.Value);

        if (request.To.HasValue)
            query = query.Where(x => x.OccurredAt <= request.To.Value);

        var count = await query.CountAsync(cancellationToken);
        var items = await query.OrderByDescending(x => x.OccurredAt)
            .ThenByDescending(x => x.Id)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new AuditLogResponse(
                x.Id, x.OccurredAt, x.ActorUserId, x.ActorUserName,
                x.EntityType, x.EntityId, x.Action, x.ChangesJson, x.IpAddress))
            .ToListAsync(cancellationToken);

        // GUID alanlarının ad/kod karşılıkları yalnızca response için üretilir.
        // AuditLogs.ChangesJson veritabanında özgün haliyle saklanmaya devam eder.
        var displayItems = await AuditReferenceDisplayResolver.ResolveAsync(
            dbContext, items, cancellationToken);

        return new PagedResult<AuditLogResponse>
        {
            Items = displayItems, TotalCount = count,
            PageNumber = request.PageNumber, PageSize = request.PageSize
        };
    }
}
