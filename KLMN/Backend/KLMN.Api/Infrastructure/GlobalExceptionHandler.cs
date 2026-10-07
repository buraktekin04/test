using FluentValidation;
using KLMN.Application.Common.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Api.Infrastructure;

/// <summary>
/// Uygulama exception'larını standart ProblemDetails response'larına dönüştürür.
/// </summary>
public sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger)
    : IExceptionHandler
{
    /// <inheritdoc />
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (
            statusCode,
            title,
            detail) =
            exception switch
            {
                ValidationException =>
                    (
                        StatusCodes.Status400BadRequest,
                        "Validation Error",
                        exception.Message
                    ),

                InvalidCurrentPasswordException =>
                    (
                        StatusCodes.Status400BadRequest,
                        "Invalid Current Password",
                        exception.Message
                    ),

                InvalidPasswordResetTokenException =>
                    (
                        StatusCodes.Status400BadRequest,
                        "Invalid Password Reset Token",
                        exception.Message
                    ),

                AccountLockedException =>
                    (
                        StatusCodes.Status423Locked,
                        "Account Locked",
                        exception.Message
                    ),

                LoginFailedException or
                AuthenticationRequiredException or
                InvalidRefreshTokenException =>
                    (
                        StatusCodes.Status401Unauthorized,
                        "Unauthorized",
                        exception.Message
                    ),

                DbUpdateConcurrencyException =>
                    (
                        StatusCodes.Status409Conflict,
                        "Concurrency Conflict",
                        "Kayıt başka bir kullanıcı tarafından değiştirilmiş olabilir. Güncel veriyi alıp tekrar deneyiniz."
                    ),

                _ =>
                    (
                        StatusCodes.Status500InternalServerError,
                        "Server Error",
                        "Beklenmeyen bir sunucu hatası oluştu."
                    )
            };

        if (statusCode >= 500)
        {
            logger.LogError(
                exception,
                "Unhandled exception. TraceId: {TraceId}",
                httpContext.TraceIdentifier);
        }
        else
        {
            logger.LogWarning(
                exception,
                "Handled application exception. TraceId: {TraceId}",
                httpContext.TraceIdentifier);
        }

        httpContext.Response.StatusCode =
            statusCode;

        var problemDetails =
            new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = detail,
                Instance =
                    httpContext.Request.Path
            };

        problemDetails.Extensions["traceId"] =
            httpContext.TraceIdentifier;

        await httpContext.Response
            .WriteAsJsonAsync(
                problemDetails,
                cancellationToken);

        return true;
    }
}
