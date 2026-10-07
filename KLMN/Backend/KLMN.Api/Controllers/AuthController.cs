using KLMN.Api.Contracts.Auth;
using KLMN.Application.Authentication.Commands.ChangePassword;
using KLMN.Application.Authentication.Commands.ForgotPassword;
using KLMN.Application.Authentication.Commands.Login;
using KLMN.Application.Authentication.Commands.Logout;
using KLMN.Application.Authentication.Commands.LogoutAll;
using KLMN.Application.Authentication.Commands.RefreshToken;
using KLMN.Application.Authentication.Commands.ResetPassword;
using KLMN.Application.Authentication.Models;
using KLMN.Application.Authentication.Queries.GetMe;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KLMN.Api.Controllers;

/// <summary>
/// KLMN authentication endpointlerini sağlar.
/// </summary>
[ApiController]
[Route("api/auth")]
[Authorize]
public sealed class AuthController(
    ISender sender,
    IWebHostEnvironment environment)
    : ControllerBase
{
    private const string RefreshTokenCookieName =
        "KLMN.RefreshToken";

    /// <summary>
    /// Kullanıcı adı/e-posta ve parola ile giriş yapar.
    /// </summary>
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType<AuthSessionResponse>(
        StatusCodes.Status200OK)]
    public async Task<ActionResult<AuthSessionResponse>> Login(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        var result =
            await sender.Send(
                new LoginCommand(
                    request.UserNameOrEmail,
                    request.Password,
                    GetIpAddress(),
                    Request.Headers.UserAgent.ToString()),
                cancellationToken);

        SetRefreshTokenCookie(
            result.RefreshToken,
            result.RefreshTokenExpiresAt);

        return Ok(result.Response);
    }

    /// <summary>
    /// HttpOnly refresh cookie üzerinden session'ı yeniler.
    /// </summary>
    [AllowAnonymous]
    [HttpPost("refresh")]
    [ProducesResponseType<AuthSessionResponse>(
        StatusCodes.Status200OK)]
    public async Task<ActionResult<AuthSessionResponse>> Refresh(
        CancellationToken cancellationToken)
    {
        if (!Request.Cookies.TryGetValue(
                RefreshTokenCookieName,
                out var refreshToken) ||
            string.IsNullOrWhiteSpace(refreshToken))
        {
            return Unauthorized();
        }

        var result =
            await sender.Send(
                new RefreshTokenCommand(
                    refreshToken,
                    GetIpAddress(),
                    Request.Headers.UserAgent.ToString()),
                cancellationToken);

        SetRefreshTokenCookie(
            result.RefreshToken,
            result.RefreshTokenExpiresAt);

        return Ok(result.Response);
    }

    /// <summary>
    /// Mevcut cihazdaki refresh session'ını sonlandırır.
    /// </summary>
    [HttpPost("logout")]
    [ProducesResponseType(
        StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Logout(
        CancellationToken cancellationToken)
    {
        Request.Cookies.TryGetValue(
            RefreshTokenCookieName,
            out var refreshToken);

        await sender.Send(
            new LogoutCommand(
                refreshToken,
                GetIpAddress()),
            cancellationToken);

        DeleteRefreshTokenCookie();

        return NoContent();
    }

    /// <summary>
    /// Kullanıcının tüm cihazlardaki session'larını sonlandırır.
    /// </summary>
    [HttpPost("logout-all")]
    [ProducesResponseType(
        StatusCodes.Status204NoContent)]
    public async Task<IActionResult> LogoutAll(
        CancellationToken cancellationToken)
    {
        await sender.Send(
            new LogoutAllCommand(),
            cancellationToken);

        DeleteRefreshTokenCookie();

        return NoContent();
    }

    /// <summary>
    /// Authenticated kullanıcının güncel profil ve authorization bilgisini döndürür.
    /// </summary>
    [HttpGet("me")]
    [ProducesResponseType<AuthUserResponse>(
        StatusCodes.Status200OK)]
    public async Task<ActionResult<AuthUserResponse>> Me(
        CancellationToken cancellationToken)
    {
        var response =
            await sender.Send(
                new GetMeQuery(),
                cancellationToken);

        return Ok(response);
    }

    /// <summary>
    /// Authenticated kullanıcının parolasını değiştirir.
    /// </summary>
    [HttpPost("change-password")]
    [ProducesResponseType(
        StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ChangePassword(
        ChangePasswordRequest request,
        CancellationToken cancellationToken)
    {
        await sender.Send(
            new ChangePasswordCommand(
                request.CurrentPassword,
                request.NewPassword,
                request.ConfirmPassword),
            cancellationToken);

        DeleteRefreshTokenCookie();

        return NoContent();
    }

    /// <summary>
    /// Kullanıcı varlığını dışarı sızdırmadan parola sıfırlama bağlantısı talep eder.
    /// </summary>
    [AllowAnonymous]
    [HttpPost("forgot-password")]
    [ProducesResponseType(
        StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ForgotPassword(
        ForgotPasswordRequest request,
        CancellationToken cancellationToken)
    {
        await sender.Send(
            new ForgotPasswordCommand(
                request.Email),
            cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Reset token ile yeni parola belirler.
    /// </summary>
    [AllowAnonymous]
    [HttpPost("reset-password")]
    [ProducesResponseType(
        StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ResetPassword(
        ResetPasswordRequest request,
        CancellationToken cancellationToken)
    {
        await sender.Send(
            new ResetPasswordCommand(
                request.Token,
                request.NewPassword,
                request.ConfirmPassword),
            cancellationToken);

        DeleteRefreshTokenCookie();

        return NoContent();
    }

    /// <summary>
    /// Refresh token değerini HttpOnly cookie olarak istemciye yazar.
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
                Secure = !environment.IsDevelopment(),
                SameSite = SameSiteMode.Lax,
                Expires = expiresAt,
                Path = "/api/auth"
            });
    }

    /// <summary>
    /// Refresh token cookie'sini mevcut path/ayarlarla siler.
    /// </summary>
    private void DeleteRefreshTokenCookie()
    {
        Response.Cookies.Delete(
            RefreshTokenCookieName,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = !environment.IsDevelopment(),
                SameSite = SameSiteMode.Lax,
                Path = "/api/auth"
            });
    }

    private string? GetIpAddress()
    {
        return HttpContext
            .Connection
            .RemoteIpAddress?
            .ToString();
    }
}
