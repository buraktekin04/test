using KLMN.Application.Common.Exceptions;
using KLMN.Application.Common.Interfaces.Identity;
using KLMN.Application.Common.Interfaces.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Authentication.Commands.ChangePassword;

/// <summary>
/// Mevcut parolayı doğrular, yeni parolayı kaydeder, SecurityStamp'i yeniler
/// ve bütün refresh token oturumlarını sonlandırır.
/// </summary>
public sealed class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand>
{
    /// <summary>
    /// Kullanıcı, yetki ve oturum verilerine erişen EF Core context sözleşmesidir.
    /// </summary>
    private readonly IKLMNDbContext _dbContext;
    /// <summary>
    /// İsteği gerçekleştiren kullanıcının kimliğini ve istemci bilgilerini sağlar.
    /// </summary>
    private readonly ICurrentUserService _currentUserService;
    /// <summary>
    /// Parola karşılaştırma ve güvenli hash üretme hizmetidir.
    /// </summary>
    private readonly IPasswordHasherService _passwordHasherService;
    /// <summary>
    /// UTC saatini test edilebilir biçimde sağlayan zaman kaynağıdır.
    /// </summary>
    private readonly TimeProvider _timeProvider;

    /// <summary>
    /// change password command handler işlemini uygulamanın ilgili kurallarına göre gerçekleştirir.
    /// </summary>
    public ChangePasswordCommandHandler(
        IKLMNDbContext dbContext,
        ICurrentUserService currentUserService,
        IPasswordHasherService passwordHasherService,
        TimeProvider timeProvider)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _passwordHasherService = passwordHasherService;
        _timeProvider = timeProvider;
    }

    /// <summary>
    /// Mevcut parolayı doğrulayıp yeni hash ile kaydeder ve oturumları iptal eder.
    /// </summary>
    public async Task Handle(
        ChangePasswordCommand request,
        CancellationToken cancellationToken)
    {
        // Kimliği doğrulanmış veya hedef kullanıcının benzersiz kimliğidir.
        var userId = _currentUserService.UserId
            ?? throw new AuthenticationRequiredException();

        // İşlem yapılacak kullanıcı hesabının takip edilen EF Core kaydıdır.
        var user = await _dbContext.Users
            .SingleOrDefaultAsync(x => x.Id == userId, cancellationToken)
            ?? throw new AuthenticationRequiredException();

        if (!_passwordHasherService.VerifyPassword(
                user.PasswordHash,
                request.CurrentPassword))
        {
            throw new InvalidCurrentPasswordException();
        }

        // İşlem sırasında tüm tarih karşılaştırmalarında kullanılacak UTC zamanıdır.
        var utcNow = _timeProvider.GetUtcNow().UtcDateTime;

        user.PasswordHash = _passwordHasherService.HashPassword(request.NewPassword);
        user.PasswordChangedDate = utcNow;
        user.SecurityStamp = Guid.NewGuid().ToString("N");

        // refresh tokens değerini mevcut işlemin sonraki kontrollerinde kullanmak üzere hesaplar.
        var refreshTokens = await _dbContext.RefreshTokens
            .IgnoreQueryFilters()
            .Where(x => x.UserId == userId && !x.RevokedAt.HasValue)
            .ToListAsync(cancellationToken);

        foreach (var token in refreshTokens)
        {
            token.RevokedAt = utcNow;
            token.RevokedByIp = _currentUserService.IpAddress;
            token.RevocationReason = "Password changed.";
            token.IsActive = false;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
