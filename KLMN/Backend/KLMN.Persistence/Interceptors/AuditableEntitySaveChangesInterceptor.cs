using KLMN.Application.Common.Interfaces.Identity;
using KLMN.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace KLMN.Persistence.Interceptors;

/// <summary>
/// BaseEntity audit alanlarını otomatik yönetir ve fiziksel delete işlemini
/// soft delete'e dönüştürür.
/// </summary>
public sealed class AuditableEntitySaveChangesInterceptor(
    ICurrentUserService currentUserService)
    : SaveChangesInterceptor
{
    /// <inheritdoc />
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        ApplyAuditInformation(eventData.Context);

        return base.SavingChanges(
            eventData,
            result);
    }

    /// <inheritdoc />
    public override ValueTask<InterceptionResult<int>>
        SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
    {
        ApplyAuditInformation(eventData.Context);

        return base.SavingChangesAsync(
            eventData,
            result,
            cancellationToken);
    }

    private void ApplyAuditInformation(
        DbContext? context)
    {
        if (context is null)
        {
            return;
        }

        var now = DateTime.UtcNow;
        var userId = currentUserService.UserId;

        var entries =
            context.ChangeTracker
                .Entries<BaseEntity>();

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedDate = now;
                    entry.Entity.CreatedBy = userId;
                    entry.Entity.IsDeleted = false;
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedDate = now;
                    entry.Entity.UpdatedBy = userId;

                    entry.Property(x => x.CreatedDate)
                        .IsModified = false;

                    entry.Property(x => x.CreatedBy)
                        .IsModified = false;
                    break;

                case EntityState.Deleted:
                    entry.State = EntityState.Modified;

                    entry.Entity.IsDeleted = true;
                    entry.Entity.IsActive = false;
                    entry.Entity.DeletedDate = now;
                    entry.Entity.DeletedBy = userId;
                    entry.Entity.UpdatedDate = now;
                    entry.Entity.UpdatedBy = userId;
                    break;
            }
        }
    }
}
