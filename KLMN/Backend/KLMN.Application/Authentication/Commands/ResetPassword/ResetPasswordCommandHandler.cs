using KLMN.Application.Common.Exceptions;
using KLMN.Application.Common.Interfaces.Authentication;
using KLMN.Application.Common.Interfaces.Identity;
using KLMN.Application.Common.Interfaces.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Authentication.Commands.ResetPassword;

/// <summary>
/// Reset token'ı doğrular, parolayı değiştirir ve mevcut authentication
/// oturumlarını geçersiz hale getirir.
/// </summary>
public sealed class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand>
{
    /// <summary>
    /// EF Core üzerinden veriye erişimi sağlayan Application katmanı veritabanı sözleşmesidir.
    /// </summary>
    private readonly IKLMNDbContext _dbContext;
    /// <summary>
    /// Parola sıfırlama tokenı oluşturur ve hash değerini hesaplar.
    /// </summary>
    private readonly IPasswordResetTokenService _tokenService;
    /// <summary>
    /// Parola hashleme ve hash doğrulaması işlemlerinde kullanılır.
    /// </summary>
    private readonly IPasswordHasherService _passwordHasherService;
    /// <summary>
    /// UTC zamanını sistem saatine doğrudan bağımlı olmadan sağlar.
    /// </summary>
    private readonly TimeProvider _timeProvider;

    /// <summary>
    /// reset password command handler işlemini uygulama kurallarına göre gerçekleştirir.
    /// </summary>
    public ResetPasswordCommandHandler(
        IKLMNDbContext dbContext,
        IPasswordResetTokenService tokenService,
        IPasswordHasherService passwordHasherService,
        TimeProvider timeProvider)
    {
        _dbContext = dbContext;
        _tokenService = tokenService;
        _passwordHasherService = passwordHasherService;
        _timeProvider = timeProvider;
    }

    /// <summary>
    /// Tokenı doğrular; kullanıcı parolasını güncelleyip eski oturumları iptal eder.
    /// </summary>
    public async Task Handle(
        ResetPasswordCommand request,
        CancellationToken cancellationToken)
    {
        // Hesap geçerliliği ve token süreleri için ortak UTC zaman değeridir.
        var utcNow = _timeProvider.GetUtcNow().UtcDateTime;
        // Açık tokenın karşılaştırma için üretilmiş SHA-256 hash değeridir.
        var tokenHash = _tokenService.HashToken(request.Token);

        // Parola sıfırlamada doğrulanan tek kullanımlık token kaydıdır.
        var resetToken = await _dbContext.PasswordResetTokens
            .IgnoreQueryFilters()
            .Include(x => x.User)
            .SingleOrDefaultAsync(x => x.TokenHash == tokenHash, cancellationToken);

        if (resetToken is null ||
            resetToken.IsDeleted ||
            !resetToken.IsActive ||
            resetToken.UsedAt.HasValue ||
            resetToken.RevokedAt.HasValue ||
            resetToken.ExpiresAt <= utcNow)
        {
            throw new InvalidPasswordResetTokenException();
        }

        // İlgili işlemin sahibi ve güvenlik durumu kontrol edilen kullanıcıdır.
        var user = resetToken.User;

        if (user.IsDeleted || !user.IsActive)
        {
            throw new InvalidPasswordResetTokenException();
        }

        user.PasswordHash = _passwordHasherService.HashPassword(request.NewPassword);
        user.PasswordChangedDate = utcNow;
        user.SecurityStamp = Guid.NewGuid().ToString("N");
        user.AccessFailedCount = 0;

        if (user.IsLocked && user.LockoutEnd.HasValue)
        {
            user.IsLocked = false;
            user.LockoutEnd = null;
        }

        resetToken.UsedAt = utcNow;
        resetToken.IsActive = false;

        // Aynı hesaba ait ve tekrar kullanımını engellemek için iptal edilen tokenlardır.
        var otherResetTokens = await _dbContext.PasswordResetTokens
            .IgnoreQueryFilters()
            .Where(x =>
                x.UserId == user.Id &&
                x.Id != resetToken.Id &&
                !x.UsedAt.HasValue &&
                !x.RevokedAt.HasValue)
            .ToListAsync(cancellationToken);

        foreach (var token in otherResetTokens)
        {
            token.RevokedAt = utcNow;
            token.IsActive = false;
        }

        // İptal edilmesi gereken kullanıcı oturumlarının listesidir.
        var refreshTokens = await _dbContext.RefreshTokens
            .IgnoreQueryFilters()
            .Where(x => x.UserId == user.Id && !x.RevokedAt.HasValue)
            .ToListAsync(cancellationToken);

        foreach (var token in refreshTokens)
        {
            token.RevokedAt = utcNow;
            token.RevocationReason = "Password reset.";
            token.IsActive = false;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
