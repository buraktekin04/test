using System.Diagnostics;
using FluentValidation;
using KLMN.Application.Common.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Api.ExceptionHandling;

/// <summary>Exception'ları standart ProblemDetails response'larına dönüştürür.</summary>
public sealed class GlobalExceptionHandler : IExceptionHandler
{
    /// <summary>
    /// Beklenen ve beklenmeyen hataları merkezi uygulama loguna kaydeder.
    /// </summary>
    private readonly ILogger<GlobalExceptionHandler> _logger;

    /// <summary>
    /// global exception handler işlemini ilgili katmanın sorumluluğuna göre gerçekleştirir.
    /// </summary>
    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Uygulama istisnalarını yakalayıp uygun HTTP cevabını üretir ve loglar.
    /// </summary>
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        // Hata türüne uygun HTTP ProblemDetails JSON gövdesidir.
        var problemDetails = CreateProblemDetails(httpContext, exception);

        if (problemDetails.Status is >= 500)
        {
            _logger.LogError(
                exception,
                "Beklenmeyen uygulama hatası. TraceId: {TraceId}",
                httpContext.TraceIdentifier);
        }
        else
        {
            _logger.LogWarning(
                "İstek başarısız. Exception: {ExceptionType}, Status: {StatusCode}, TraceId: {TraceId}",
                exception.GetType().Name,
                problemDetails.Status,
                httpContext.TraceIdentifier);
        }

        httpContext.Response.StatusCode =
            problemDetails.Status ?? StatusCodes.Status500InternalServerError;

        await httpContext.Response.WriteAsJsonAsync(
            problemDetails,
            cancellationToken);

        return true;
    }

    /// <summary>
    /// Hata tipini kullanıcıya güvenli standart HTTP ProblemDetails sonucuna dönüştürür.
    /// </summary>
    private static ProblemDetails CreateProblemDetails(
        HttpContext httpContext,
        Exception exception)
    {
        // problem değerini sonraki işlem için hesaplar.
        var problem = exception switch
        {
            ValidationException validation =>
                CreateValidationProblem(validation),

            InvalidCurrentPasswordException =>
                new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Mevcut parola geçersiz.",
                    Detail = exception.Message
                },

            InvalidPasswordResetTokenException =>
                new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Parola sıfırlama bağlantısı geçersiz.",
                    Detail = exception.Message
                },

            LoginFailedException =>
                new ProblemDetails
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Title = "Giriş başarısız.",
                    Detail = exception.Message
                },

            InvalidRefreshTokenException or AuthenticationRequiredException =>
                new ProblemDetails
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Title = "Oturum geçersiz.",
                    Detail = exception.Message
                },

            AccountLockedException =>
                new ProblemDetails
                {
                    Status = StatusCodes.Status423Locked,
                    Title = "Hesap kilitli.",
                    Detail = exception.Message
                },

            DbUpdateConcurrencyException =>
                new ProblemDetails
                {
                    Status = StatusCodes.Status409Conflict,
                    Title = "Kayıt güncellenemedi.",
                    Detail = "Kayıt başka bir kullanıcı tarafından güncellenmiştir. Güncel veriyi tekrar yükleyiniz."
                },

            _ =>
                new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "Beklenmeyen bir hata oluştu.",
                    Detail = "İşlem sırasında beklenmeyen bir hata oluştu."
                }
        };

        problem.Instance = httpContext.Request.Path;
        problem.Extensions["traceId"] =
            Activity.Current?.Id ?? httpContext.TraceIdentifier;

        return problem;
    }

    /// <summary>
    /// FluentValidation hatalarını ilgili özellik adlarına göre ProblemDetails'a dönüştürür.
    /// </summary>
    private static ProblemDetails CreateValidationProblem(
        ValidationException exception)
    {
        // Validation hatalarının alanlara göre gruplanmış mesaj sözlüğüdür.
        var errors = exception.Errors
            .GroupBy(x => x.PropertyName)
            .ToDictionary(
                x => x.Key,
                x => x.Select(y => y.ErrorMessage).Distinct().ToArray());

        // problem değerini sonraki işlem için hesaplar.
        var problem = new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Doğrulama hatası.",
            Detail = "Gönderilen bilgilerden biri veya birkaçı geçersizdir."
        };

        problem.Extensions["errors"] = errors;
        return problem;
    }
}
