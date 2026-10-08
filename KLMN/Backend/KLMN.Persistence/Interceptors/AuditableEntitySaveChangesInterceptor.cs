using KLMN.Application.Common.Interfaces.Identity;
using KLMN.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace KLMN.Persistence.Interceptors;

/// <summary>Audit alanlarını yönetir ve fiziksel delete'i soft delete'e dönüştürür.</summary>
public sealed class AuditableEntitySaveChangesInterceptor : SaveChangesInterceptor
{
    /// <summary>
    /// Oluşturma, değiştirme ve silme audit'ini yapan kullanıcı bilgisini sağlar.
    /// </summary>
    private readonly ICurrentUserService _currentUserService;

    /// <summary>
    /// auditable entity save changes interceptor işlemini ilgili persistence sorumluluğuyla yerine getirir.
    /// </summary>
    public AuditableEntitySaveChangesInterceptor(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    /// <summary>
    /// saving changes işlemini ilgili persistence sorumluluğuyla yerine getirir.
    /// </summary>
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        if (eventData.Context is not null)
        {
            ApplyAuditInformation(eventData.Context);
        }

        return base.SavingChanges(eventData, result);
    }

    /// <summary>
    /// saving changes async işlemini ilgili persistence sorumluluğuyla yerine getirir.
    /// </summary>
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
        {
            ApplyAuditInformation(eventData.Context);
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    /// <summary>
    /// ChangeTracker'daki ekleme, değiştirme ve silme durumlarını audit ile soft delete'e dönüştürür.
    /// </summary>
    private void ApplyAuditInformation(DbContext dbContext)
    {
        // Audit alanlarında kullanılacak tutarlı UTC zamanıdır.
        var utcNow = DateTime.UtcNow;
        // Değişiklik kaydına yazılacak geçerli kullanıcı kimliğidir.
        var currentUserId = _currentUserService.UserId;

        // SaveChanges sırasında eklenen, değiştirilen ve silinen BaseEntity kayıtlarıdır.
        var entries = dbContext.ChangeTracker
            .Entries<BaseEntity>()
            .Where(x => x.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .ToList();

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedDate = utcNow;
                    entry.Entity.CreatedBy = currentUserId;
                    entry.Entity.IsActive = true;
                    entry.Entity.IsDeleted = false;
                    entry.Entity.UpdatedDate = null;
                    entry.Entity.UpdatedBy = null;
                    entry.Entity.DeletedDate = null;
                    entry.Entity.DeletedBy = null;
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedDate = utcNow;
                    entry.Entity.UpdatedBy = currentUserId;
                    entry.Property(x => x.CreatedDate).IsModified = false;
                    entry.Property(x => x.CreatedBy).IsModified = false;
                    break;

                case EntityState.Deleted:
                    entry.State = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                    entry.Entity.IsActive = false;
                    entry.Entity.DeletedDate = utcNow;
                    entry.Entity.DeletedBy = currentUserId;
                    entry.Entity.UpdatedDate = utcNow;
                    entry.Entity.UpdatedBy = currentUserId;
                    break;
            }
        }
    }
}
