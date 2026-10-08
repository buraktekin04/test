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
using KLMN.Application.Common.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KLMN.Api.Controllers;

/// <summary>
/// Kullanıcı authentication işlemlerini sağlayan API endpoint'lerini içerir.
/// </summary>
[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private const string RefreshTokenCookieName =
        "klmn_refresh_token";

    private readonly ISender _sender;
    private readonly IWebHostEnvironment _environment;

    /// <summary>
    /// AuthController sınıfının yeni örneğini oluşturur.
    /// </summary>
    /// <param name="sender">
    /// CQRS command ve query işlemlerini MediatR üzerinden çalıştırır.
    /// </param>
    /// <param name="environment">
    /// Uygulamanın development/production ortam bilgisini sağlar.
    /// </param>
    public AuthController(
        ISender sender,
        IWebHostEnvironment environment)
    {
        _sender = sender;
        _environment = environment;
    }

    /// <summary>
    /// Kullanıcı adı/e-posta ve parola ile sisteme giriş yapar.
    /// Başarılı girişte access token response içerisinde,
    /// refresh token ise HttpOnly cookie içerisinde gönderilir.
    /// </summary>
    /// <param name="request">
    /// Kullanıcının giriş bilgileridir.
    /// </param>
    /// <param name="cancellationToken">
    /// İstek iptal token'ıdır.
    /// </param>
    /// <returns>
    /// Access token ve kullanıcı bilgilerini döndürür.
    /// </returns>
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType<AuthSessionResponse>(
        StatusCodes.Status200OK)]
    public async Task<ActionResult<AuthSessionResponse>> Login(
        [FromBody] LoginCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            command,
            cancellationToken);

        SetRefreshTokenCookie(
            result.RefreshToken,
            result.RefreshTokenExpiresAt);

        return Ok(result.Response);
    }

    /// <summary>
    /// HttpOnly refresh cookie kullanılarak yeni access token
    /// ve refresh token üretir.
    /// </summary>
    /// <param name="cancellationToken">
    /// İstek iptal token'ıdır.
    /// </param>
    /// <returns>
    /// Yenilenmiş authentication session bilgisini döndürür.
    /// </returns>
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
            throw new InvalidRefreshTokenException();
        }

        var result = await _sender.Send(
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
    /// Mevcut refresh token oturumunu revoke eder ve
    /// refresh token cookie'sini istemciden kaldırır.
    /// </summary>
    /// <param name="cancellationToken">
    /// İstek iptal token'ıdır.
    /// </param>
    [Authorize]
    [HttpPost("logout")]
    [ProducesResponseType(
        StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Logout(
        CancellationToken cancellationToken)
    {
        Request.Cookies.TryGetValue(
            RefreshTokenCookieName,
            out var refreshToken);

        await _sender.Send(
            new LogoutCommand(
                refreshToken,
                GetIpAddress()),
            cancellationToken);

        DeleteRefreshTokenCookie();

        return NoContent();
    }

    /// <summary>
    /// Kimliği doğrulanmış mevcut kullanıcının güncel profil,
    /// rol ve permission bilgilerini döndürür.
    /// </summary>
    /// <param name="cancellationToken">
    /// İstek iptal token'ıdır.
    /// </param>
    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType<AuthUserResponse>(
        StatusCodes.Status200OK)]
    public async Task<ActionResult<AuthUserResponse>> Me(
        CancellationToken cancellationToken)
    {
        var response = await _sender.Send(
            new GetMeQuery(),
            cancellationToken);

        return Ok(response);
    }

    /// <summary>
    /// Kimliği doğrulanmış kullanıcının parolasını değiştirir.
    /// Parola değişikliği başarılı olduğunda aktif refresh session'ları
    /// command handler tarafından sonlandırılır ve mevcut cookie silinir.
    /// </summary>
    /// <param name="request">
    /// Mevcut ve yeni parola bilgileridir.
    /// </param>
    /// <param name="cancellationToken">
    /// İstek iptal token'ıdır.
    /// </param>
    [Authorize]
    [HttpPost("change-password")]
    [ProducesResponseType(
        StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordRequest request,
        CancellationToken cancellationToken)
    {
        await _sender.Send(
            new ChangePasswordCommand(
                request.CurrentPassword,
                request.NewPassword,
                request.ConfirmPassword),
            cancellationToken);

        DeleteRefreshTokenCookie();

        return NoContent();
    }

    /// <summary>
    /// Kullanıcının bütün cihazlardaki oturumlarını sonlandırır.
    /// </summary>
    /// <param name="cancellationToken">
    /// İstek iptal token'ıdır.
    /// </param>
    [Authorize]
    [HttpPost("logout-all")]
    [ProducesResponseType(
        StatusCodes.Status204NoContent)]
    public async Task<IActionResult> LogoutAll(
        CancellationToken cancellationToken)
    {
        await _sender.Send(
            new LogoutAllCommand(),
            cancellationToken);

        DeleteRefreshTokenCookie();

        return NoContent();
    }

    /// <summary>
    /// Kullanıcının e-posta adresine parola sıfırlama bağlantısı gönderir.
    /// Kullanıcı bulunmadığı güvenlik amacıyla response'ta açıklanmaz.
    /// </summary>
    /// <param name="request">
    /// Parola sıfırlama talebidir.
    /// </param>
    /// <param name="cancellationToken">
    /// İstek iptal token'ıdır.
    /// </param>
    [AllowAnonymous]
    [HttpPost("forgot-password")]
    [ProducesResponseType(
        StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ForgotPassword(
        [FromBody] ForgotPasswordRequest request,
        CancellationToken cancellationToken)
    {
        await _sender.Send(
            new ForgotPasswordCommand(
                request.Email),
            cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Geçerli parola sıfırlama token'ını kullanarak yeni parola belirler.
    /// </summary>
    /// <param name="request">
    /// Reset token ve yeni parola bilgileridir.
    /// </param>
    /// <param name="cancellationToken">
    /// İstek iptal token'ıdır.
    /// </param>
    [AllowAnonymous]
    [HttpPost("reset-password")]
    [ProducesResponseType(
        StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ResetPassword(
        [FromBody] ResetPasswordRequest request,
        CancellationToken cancellationToken)
    {
        await _sender.Send(
            new ResetPasswordCommand(
                request.Token,
                request.NewPassword,
                request.ConfirmPassword),
            cancellationToken);

        DeleteRefreshTokenCookie();

        return NoContent();
    }

    /// <summary>
    /// Refresh token değerini güvenli HttpOnly cookie olarak
    /// HTTP response'a ekler.
    ///
    /// Development ortamında Angular proxy kullanıldığı için cookie,
    /// localhost üzerinde Secure=false ve SameSite=Lax olarak oluşturulur.
    /// Production ortamında Secure=true olarak gönderilir.
    /// </summary>
    /// <param name="refreshToken">
    /// İstemciye gönderilecek açık refresh token değeridir.
    /// </param>
    /// <param name="expiresAt">
    /// Refresh token'ın UTC sona erme zamanıdır.
    /// </param>
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

                Secure =
                    !_environment.IsDevelopment(),

                SameSite =
                    SameSiteMode.Lax,

                Expires =
                    new DateTimeOffset(
                        DateTime.SpecifyKind(
                            expiresAt,
                            DateTimeKind.Utc)),

                IsEssential = true,

                Path = "/api/auth"
            });
    }

    /// <summary>
    /// Refresh token cookie'sini istemciden kaldırır.
    /// Cookie silinirken oluşturulduğu Path, Secure ve SameSite
    /// ayarlarıyla aynı değerler kullanılır.
    /// </summary>
    private void DeleteRefreshTokenCookie()
    {
        Response.Cookies.Delete(
            RefreshTokenCookieName,
            new CookieOptions
            {
                HttpOnly = true,

                Secure =
                    !_environment.IsDevelopment(),

                SameSite =
                    SameSiteMode.Lax,

                IsEssential = true,

                Path = "/api/auth"
            });
    }

    /// <summary>
    /// Mevcut HTTP request'in uzak IP adresini döndürür.
    /// </summary>
    /// <returns>
    /// Uzak IP adresi veya null.
    /// </returns>
    private string? GetIpAddress()
    {
        return HttpContext
            .Connection
            .RemoteIpAddress?
            .ToString();
    }
}
