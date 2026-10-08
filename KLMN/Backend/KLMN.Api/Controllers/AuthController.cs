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
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KLMN.Api.Controllers;

/// <summary>KLMN authentication endpointlerini sağlar.</summary>
[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private const string RefreshTokenCookieName = "klmn_refresh_token";

    private readonly ISender _sender;
    private readonly IWebHostEnvironment _environment;

    public AuthController(
        ISender sender,
        IWebHostEnvironment environment)
    {
        _sender = sender;
        _environment = environment;
    }

    /// <summary>Kullanıcı adı/e-posta ve parola ile giriş yapar.</summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<LoginResponse>> Login(
        [FromBody] LoginCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);

        SetRefreshTokenCookie(
            result.RefreshToken,
            result.RefreshTokenExpiresAt);

        return Ok(result.Response);
    }

    /// <summary>HttpOnly refresh cookie üzerinden session yeniler.</summary>
    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(RefreshResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<RefreshResponse>> Refresh(
        CancellationToken cancellationToken)
    {
        if (!Request.Cookies.TryGetValue(
                RefreshTokenCookieName,
                out var refreshToken) ||
            string.IsNullOrWhiteSpace(refreshToken))
        {
            throw new InvalidRefreshTokenException();
        }

        var result = await _sender.Send(
            new RefreshTokenCommand
            {
                RefreshToken = refreshToken
            },
            cancellationToken);

        SetRefreshTokenCookie(
            result.RefreshToken,
            result.RefreshTokenExpiresAt);

        return Ok(result.Response);
    }

    /// <summary>Mevcut refresh token oturumunu sonlandırır.</summary>
    [HttpPost("logout")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Logout(
        CancellationToken cancellationToken)
    {
        if (Request.Cookies.TryGetValue(
                RefreshTokenCookieName,
                out var refreshToken) &&
            !string.IsNullOrWhiteSpace(refreshToken))
        {
            await _sender.Send(
                new LogoutCommand
                {
                    RefreshToken = refreshToken
                },
                cancellationToken);
        }

        DeleteRefreshTokenCookie();
        return NoContent();
    }

    /// <summary>Authenticated kullanıcının güncel profil ve yetkilerini döndürür.</summary>
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

    /// <summary>
    /// Authenticated kullanıcının parolasını değiştirir.
    /// Başarılı işlem bütün refresh session'larını sonlandırır.
    /// </summary>
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

    /// <summary>Kullanıcının bütün cihazlardaki oturumlarını sonlandırır.</summary>
    [HttpPost("logout-all")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> LogoutAll(
        CancellationToken cancellationToken)
    {
        await _sender.Send(
            new LogoutAllCommand(),
            cancellationToken);

        DeleteRefreshTokenCookie();
        return NoContent();
    }

    /// <summary>Kullanıcı varlığını açıklamadan parola sıfırlama bağlantısı talep eder.</summary>
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

    /// <summary>Geçerli reset token ile yeni parola belirler.</summary>
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
    /// Refresh token'ı HttpOnly cookie olarak yazar.
    /// Development'ta Angular proxy nedeniyle Secure=false; production'da true kullanılır.
    /// </summary>
    private void SetRefreshTokenCookie(
        string refreshToken,
        DateTime expiresAt)
    {
        Response.Cookies.Append(
            RefreshTokenCookieName,
            refreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = !_environment.IsDevelopment(),
                SameSite = SameSiteMode.Lax,
                Expires = new DateTimeOffset(
                    DateTime.SpecifyKind(
                        expiresAt,
                        DateTimeKind.Utc)),
                IsEssential = true,
                Path = "/api/auth"
            });
    }

    /// <summary>Refresh token cookie'sini aynı cookie seçenekleriyle siler.</summary>
    private void DeleteRefreshTokenCookie()
    {
        Response.Cookies.Delete(
            RefreshTokenCookieName,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = !_environment.IsDevelopment(),
                SameSite = SameSiteMode.Lax,
                IsEssential = true,
                Path = "/api/auth"
            });
    }
}

