using KLMN.Application.Authentication.Commands.ChangePassword;
using KLMN.Application.Authentication.Commands.ForgotPassword;
using KLMN.Application.Authentication.Commands.Login;
using KLMN.Application.Authentication.Commands.Logout;
using KLMN.Application.Authentication.Commands.LogoutAll;
using KLMN.Application.Authentication.Commands.RefreshToken;
using KLMN.Application.Authentication.Commands.ResetPassword;
using KLMN.Application.Authentication.Queries.GetCurrentUser;
using KLMN.Application.Authentication.Responses;
using KLMN.Application.Common.Exceptions;
using KLMN.Application.Common.Options;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace KLMN.Api.Controllers;

/// <summary>
/// Oturum açma, cookie üzerinden refresh, çıkış ve parola yönetimi
/// endpoint'lerini sağlar. Açık refresh token response body'ye yazılmaz.
/// </summary>
[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    /// <summary>CQRS command ve query işlemlerini yürüten MediatR servisidir.</summary>
    private readonly ISender _sender;

    /// <summary>
    /// Seçili ortamın Authentication bölümünden gelen refresh cookie
    /// adı ve SameSite davranışını içerir.
    /// </summary>
    private readonly AuthenticationSettings _authenticationSettings;

    /// <summary>
    /// Cross-site cookie kullanımında CSRF Origin denetimi için
    /// Cors:AllowedOrigins değerlerini okuyan yapılandırmadır.
    /// </summary>
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Refresh oturum hatalarını açık token/hash yazmadan loglar.
    /// </summary>
    private readonly ILogger<AuthController> _logger;

    /// <summary>
    /// Controller'ın ihtiyaç duyduğu MediatR, cookie ayarı, CORS
    /// yapılandırması ve loglama servislerini bağlar.
    /// </summary>
    public AuthController(
        ISender sender,
        IOptions<AuthenticationSettings> authenticationSettings,
        IConfiguration configuration,
        ILogger<AuthController> logger)
    {
        _sender = sender;
        _authenticationSettings = authenticationSettings.Value;
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Kullanıcı adı/e-posta ve parola ile oturum açar. Yeni refresh token,
    /// ilgili ortama özgü HttpOnly cookie olarak oluşturulur.
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<LoginResponse>> Login(
        [FromBody] LoginCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);

        SetRefreshTokenCookie(result.RefreshToken, result.RefreshTokenExpiresAt);

        return Ok(result.Response);
    }

    /// <summary>
    /// Tarayıcının HttpOnly cookie değerini kullanarak access/refresh
    /// token çiftini yeniler. Eksik veya sunucuda eşleşmeyen cookie'yi
    /// temizleyerek aynı geçersiz tokenın tekrar gönderilmesini önler.
    /// </summary>
    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(RefreshResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<RefreshResponse>> Refresh(
        CancellationToken cancellationToken)
    {
        // SameSite=None kullanımında üçüncü taraf sitelerin refresh isteği
        // başlatmasını engellemek için Origin, CORS whitelist'iyle doğrulanır.
        if (!HasAllowedCookieRequestOrigin())
        {
            return StatusCode(StatusCodes.Status403Forbidden);
        }

        if (!Request.Cookies.TryGetValue(
                _authenticationSettings.RefreshCookieName,
                out var refreshToken) ||
            string.IsNullOrWhiteSpace(refreshToken))
        {
            DeleteRefreshTokenCookie();

            // Exception middleware hata işleme sırasında response header'larını
            // sıfırlayabileceği için burada exception fırlatmıyoruz; 401'i
            // doğrudan döndürerek Set-Cookie silme talimatını koruyoruz.
            return Unauthorized(CreateInvalidSessionProblem());
        }

        try
        {
            var result = await _sender.Send(
                new RefreshTokenCommand { RefreshToken = refreshToken },
                cancellationToken);

            // Token rotation başarılı olduktan sonra eski cookie'nin yerine
            // yeni tokenı yazar; böylece sonraki F5'te güncel token gönderilir.
            SetRefreshTokenCookie(result.RefreshToken, result.RefreshTokenExpiresAt);

            return Ok(result.Response);
        }
        catch (InvalidRefreshTokenException)
        {
            // Eski oturum tokenı veritabanında bulunmuyorsa temizle.
            // Geçersiz tokenı kabul ederek yeni bir oturum OLUŞTURMUYORUZ.
            DeleteRefreshTokenCookie();
            _logger.LogWarning(
                "Refresh cookie geçersiz; tarayıcıdan kaldırıldı. Cookie adı: {CookieName}",
                _authenticationSettings.RefreshCookieName);

            // Hata yanıtını doğrudan üretmek, Set-Cookie silme header'ının
            // exception handler tarafından temizlenmesini engeller.
            return Unauthorized(CreateInvalidSessionProblem());
        }
    }

    /// <summary>
    /// İsteğin refresh token cookie'sini veritabanında revoke ederek
    /// ilgili tarayıcı oturumunu sonlandırır. Logout idempotent'tir.
    /// </summary>
    [HttpPost("logout")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Logout(
        CancellationToken cancellationToken)
    {
        if (!HasAllowedCookieRequestOrigin())
        {
            return StatusCode(StatusCodes.Status403Forbidden);
        }

        if (Request.Cookies.TryGetValue(
                _authenticationSettings.RefreshCookieName,
                out var refreshToken) &&
            !string.IsNullOrWhiteSpace(refreshToken))
        {
            await _sender.Send(
                new LogoutCommand { RefreshToken = refreshToken },
                cancellationToken);
        }

        DeleteRefreshTokenCookie();
        return NoContent();
    }

    /// <summary>Kimliği doğrulanmış kullanıcının güncel profil ve yetkilerini döndürür.</summary>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(CurrentUserResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<CurrentUserResponse>> Me(
        CancellationToken cancellationToken)
    {
        var response = await _sender.Send(
            new GetCurrentUserQuery(),
            cancellationToken);

        return Ok(response);
    }

    /// <summary>Parolayı değiştirir, bütün refresh oturumlarını iptal eder.</summary>
    [HttpPost("change-password")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordCommand command,
        CancellationToken cancellationToken)
    {
        await _sender.Send(command, cancellationToken);
        DeleteRefreshTokenCookie();
        return NoContent();
    }

    /// <summary>Güvenlik damgasını yenileyerek tüm cihazlardan çıkış yapar.</summary>
    [HttpPost("logout-all")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> LogoutAll(
        CancellationToken cancellationToken)
    {
        await _sender.Send(new LogoutAllCommand(), cancellationToken);
        DeleteRefreshTokenCookie();
        return NoContent();
    }

    /// <summary>Kullanıcı varlığını belirtmeden parola sıfırlama bağlantısı talep eder.</summary>
    [HttpPost("forgot-password")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ForgotPassword(
        [FromBody] ForgotPasswordCommand command,
        CancellationToken cancellationToken)
    {
        await _sender.Send(command, cancellationToken);
        return NoContent();
    }

    /// <summary>Tek kullanımlık reset tokenıyla parola değiştirir.</summary>
    [HttpPost("reset-password")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ResetPassword(
        [FromBody] ResetPasswordCommand command,
        CancellationToken cancellationToken)
    {
        await _sender.Send(command, cancellationToken);
        DeleteRefreshTokenCookie();
        return NoContent();
    }

    /// <summary>
    /// İstemciye tokenın bulunup bulunmadığını açıklamayan standart
    /// 401 oturum hatasını üretir. Refresh token ve hash bilgisi dönülmez.
    /// </summary>
    private static ProblemDetails CreateInvalidSessionProblem() =>
        new()
        {
            Status = StatusCodes.Status401Unauthorized,
            Title = "Oturum geçersiz.",
            Detail = "Oturum bilgisi geçersiz veya süresi dolmuştur."
        };

    /// <summary>
    /// Refresh cookie'nin SameSite değerini seçili ortamın appsettings
    /// dosyasından okur. Cookie her zaman Secure/HttpOnly kalır.
    /// </summary>
    private SameSiteMode GetRefreshCookieSameSite() =>
        Enum.Parse<SameSiteMode>(
            _authenticationSettings.RefreshCookieSameSite,
            ignoreCase: true);

    /// <summary>
    /// SameSite=None durumunda gelen POST isteğinin Origin başlığını,
    /// appsettings'teki izinli frontend adresleriyle karşılaştırır.
    /// Origin yoksa veya izinli değilse cookie tabanlı işlem reddedilir.
    /// Lax/Strict kullanıldığında ek Origin zorunluluğu yoktur.
    /// </summary>
    private bool HasAllowedCookieRequestOrigin()
    {
        if (GetRefreshCookieSameSite() != SameSiteMode.None)
        {
            return true;
        }

        var origin = Request.Headers.Origin.ToString();

        if (string.IsNullOrWhiteSpace(origin))
        {
            return false;
        }

        var allowedOrigins = _configuration
            .GetSection("Cors:AllowedOrigins")
            .Get<string[]>() ?? [];

        return allowedOrigins.Contains(origin, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Başarılı giriş/refresh sonrasında açık tokenı, JavaScript'ten
    /// erişilemeyen HttpOnly, Secure ve ortamın SameSite politikasına
    /// uygun cookie içerisinde saklar.
    /// </summary>
    private void SetRefreshTokenCookie(string refreshToken, DateTime expiresAt)
    {
        Response.Cookies.Append(
            _authenticationSettings.RefreshCookieName,
            refreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = GetRefreshCookieSameSite(),
                Expires = new DateTimeOffset(
                    DateTime.SpecifyKind(expiresAt, DateTimeKind.Utc)),
                IsEssential = true,
                Path = "/api/auth"
            });
    }

    /// <summary>
    /// Cookie'yi oluştururken kullanılan isim, path, SameSite ve
    /// Secure seçenekleriyle siler. DB'deki eski tokenı geri getirmez.
    /// </summary>
    private void DeleteRefreshTokenCookie()
    {
        Response.Cookies.Delete(
            _authenticationSettings.RefreshCookieName,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = GetRefreshCookieSameSite(),
                IsEssential = true,
                Path = "/api/auth"
            });
    }
}
