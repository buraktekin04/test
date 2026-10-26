using KLMN.Application.SYYDiagnosisDecisions.Exceptions;
using KLMN.Application.HcDecisions.Exceptions;
using KLMN.Application.HcUnusedReasons.Exceptions;
using KLMN.Application.HcUnusedAdmissionTypes.Exceptions;
using KLMN.Application.HcUnusedDecisions.Exceptions;
using KLMN.Application.HcDecisionConnections.Exceptions;
using KLMN.Application.ICD10Defs.Exceptions;
using KLMN.Application.ICD10Diagnostics.Exceptions;
using KLMN.Application.SYYClassifications.Exceptions;
using KLMN.Application.SYYDiagnosisBranches.Exceptions;
using KLMN.Application.SYYDiagnosisProcedures.Exceptions;
using KLMN.Application.SYYDiagnosisRequireds.Exceptions;
using KLMN.Application.SYYDiagnoses.Exceptions;
using KLMN.Application.SYYDiagnosisTypes.Exceptions;
using KLMN.Application.SYYDiagnosisSlices.Exceptions;
using KLMN.Application.AnnotationReasons.Exceptions;
using KLMN.Application.PlaneTypes.Exceptions;
using KLMN.Application.FlyingMissions.Exceptions;
using KLMN.Application.ProcedureTypes.Exceptions;
using KLMN.Application.ReasonForExamBranchDefs.Exceptions;
using KLMN.Application.ReasonForExamProcedureDefs.Exceptions;
using KLMN.Application.ReasonForExamUnusedHcAdmissionTypes.Exceptions;
using KLMN.Application.ReasonForExamReportTypes.Exceptions;
using KLMN.Application.Procedures.Exceptions;
using KLMN.Application.ReasonForExaminations.Exceptions;
using KLMN.Application.ForcesCommands.Exceptions;
using KLMN.Application.Countries.Exceptions;
using KLMN.Application.Cities.Exceptions;
using KLMN.Application.MilitarySchools.Exceptions;
using KLMN.Application.MilitaryOffices.Exceptions;
using KLMN.Application.AdmissionTypes.Exceptions;
using KLMN.Application.ConnectedDecisions.Exceptions;
using KLMN.Application.Branches.Exceptions;
using KLMN.Application.ReportTypes.Exceptions;
using KLMN.Application.Hospitals.Exceptions;
using KLMN.Application.HospitalReportTypes.Exceptions;
using KLMN.Application.RankGroups.Exceptions;
using KLMN.Application.Ranks.Exceptions;
using KLMN.Application.MilitaryClasses.Exceptions;
using KLMN.Application.MilitaryClassGroups.Exceptions;
using KLMN.Application.MilitaryClassGroupRelations.Exceptions;
using KLMN.Application.Towns.Exceptions;
using System.Diagnostics;
using FluentValidation;
using KLMN.Application.Common.Exceptions;
using KLMN.Application.Users.Exceptions;
using KLMN.Application.Roles.Exceptions;
using KLMN.Application.MilitaryUnits.Exceptions;
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

            UserNotFoundException =>
                new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "Kullanıcı bulunamadı.",
                    Detail = exception.Message
                },

            UserRoleAssignmentForbiddenException =>
                new ProblemDetails
                {
                    Status = StatusCodes.Status403Forbidden,
                    Title = "Rol atama işlemi reddedildi.",
                    Detail = exception.Message
                },

            UserConflictException =>
                new ProblemDetails
                {
                    Status = StatusCodes.Status409Conflict,
                    Title = "Kullanıcı işlemi gerçekleştirilemedi.",
                    Detail = exception.Message
                },

            RoleNotFoundException =>
                new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "Rol bulunamadı.",
                    Detail = exception.Message
                },

            RoleConflictException =>
                new ProblemDetails
                {
                    Status = StatusCodes.Status409Conflict,
                    Title = "Rol işlemi gerçekleştirilemedi.",
                    Detail = exception.Message
                },

            ForcesCommandNotFoundException =>
                new ProblemDetails { Status = StatusCodes.Status404NotFound, Title = "Kuvvet komutanlığı bulunamadı.", Detail = exception.Message },
            ForcesCommandConflictException =>
                new ProblemDetails { Status = StatusCodes.Status409Conflict, Title = "Kuvvet komutanlığı işlemi gerçekleştirilemedi.", Detail = exception.Message },
            CountryNotFoundException =>
                new ProblemDetails { Status = StatusCodes.Status404NotFound,
                    Title = "Ülke bulunamadı.", Detail = exception.Message },
            CountryConflictException =>
                new ProblemDetails { Status = StatusCodes.Status409Conflict,
                    Title = "Ülke işlemi gerçekleştirilemedi.", Detail = exception.Message },
            TownNotFoundException =>
                new ProblemDetails { Status = StatusCodes.Status404NotFound,
                    Title = "İlçe bulunamadı.", Detail = exception.Message },
            TownConflictException =>
                new ProblemDetails { Status = StatusCodes.Status409Conflict,
                    Title = "İlçe işlemi gerçekleştirilemedi.", Detail = exception.Message },
            MilitaryClassGroupNotFoundException => new ProblemDetails { Status = 404,
                Title = "Sınıf grubu bulunamadı.", Detail = exception.Message },
            MilitaryClassGroupConflictException => new ProblemDetails { Status = 409,
                Title = "Sınıf grubu işlem çakışması.", Detail = exception.Message },
            MilitaryClassGroupRelationNotFoundException => new ProblemDetails { Status = 404,
                Title = "Eşleştirme bulunamadı.", Detail = exception.Message },
            MilitaryClassGroupRelationConflictException => new ProblemDetails { Status = 409,
                Title = "Eşleştirme işlem çakışması.", Detail = exception.Message },
            MilitaryClassNotFoundException =>
                new ProblemDetails { Status = StatusCodes.Status404NotFound,
                    Title = "Askerî sınıf bulunamadı.", Detail = exception.Message },
            MilitaryClassConflictException =>
                new ProblemDetails { Status = StatusCodes.Status409Conflict,
                    Title = "Askerî sınıf işlemi gerçekleştirilemedi.", Detail = exception.Message },
            SYYDiagnosisDecisionNotFoundException =>
                new ProblemDetails { Status=StatusCodes.Status404NotFound,Title="SYY tanı karar istatistiği bulunamadı.",Detail=exception.Message },
            SYYDiagnosisDecisionConflictException =>
                new ProblemDetails { Status=StatusCodes.Status409Conflict,Title="SYY tanı karar istatistiği kaydedilemedi.",Detail=exception.Message },
            HcDecisionDefNotFoundException => new ProblemDetails { Status=404, Title="Kurul Karar Tanımları - kayıt bulunamadı.", Detail=exception.Message },
            HcDecisionDefConflictException => new ProblemDetails { Status=409, Title="Kurul Karar Tanımları - işlem engellendi.", Detail=exception.Message },
            HcUnusedReasonForExamNotFoundException => new ProblemDetails { Status=404, Title="Kullanılmayacak Muayene Sebepleri - kayıt bulunamadı.", Detail=exception.Message },
            HcUnusedReasonForExamConflictException => new ProblemDetails { Status=409, Title="Kullanılmayacak Muayene Sebepleri - işlem engellendi.", Detail=exception.Message },
            HcUnusedAdmissionTypeNotFoundException => new ProblemDetails { Status=404, Title="Kullanılmayacak Başvuru Grupları - kayıt bulunamadı.", Detail=exception.Message },
            HcUnusedAdmissionTypeConflictException => new ProblemDetails { Status=409, Title="Kullanılmayacak Başvuru Grupları - işlem engellendi.", Detail=exception.Message },
            HcUnusedDecisionNotFoundException => new ProblemDetails { Status=404, Title="Birlikte Verilmeyecek Kararlar - kayıt bulunamadı.", Detail=exception.Message },
            HcUnusedDecisionConflictException => new ProblemDetails { Status=409, Title="Birlikte Verilmeyecek Kararlar - işlem engellendi.", Detail=exception.Message },
            HcConnectedDecisionNotFoundException => new ProblemDetails { Status=404, Title="Bağlı Kararlar - kayıt bulunamadı.", Detail=exception.Message },
            HcConnectedDecisionConflictException => new ProblemDetails { Status=409, Title="Bağlı Kararlar - işlem engellendi.", Detail=exception.Message },
            ReasonForExaminationNotFoundException => new ProblemDetails { Status=404, Title="Kayıt bulunamadı.", Detail=exception.Message },
            ReasonForExaminationConflictException => new ProblemDetails { Status=409, Title="Eşleştirme işlemi gerçekleştirilemedi.", Detail=exception.Message },
            SYYClassificationDefNotFoundException => new ProblemDetails { Status=404,Title="Kayıt bulunamadı.",Detail=exception.Message },
            SYYClassificationDefConflictException => new ProblemDetails { Status=409,Title="Tanım çakışması.",Detail=exception.Message },
            SYYDiagnosisBranchDefNotFoundException => new ProblemDetails { Status=404,Title="Kayıt bulunamadı.",Detail=exception.Message },
            SYYDiagnosisBranchDefConflictException => new ProblemDetails { Status=409,Title="Tanım çakışması.",Detail=exception.Message },
            SYYDiagnosisProcedureDefNotFoundException => new ProblemDetails { Status=404,Title="Kayıt bulunamadı.",Detail=exception.Message },
            SYYDiagnosisProcedureDefConflictException => new ProblemDetails { Status=409,Title="Tanım çakışması.",Detail=exception.Message },
            SYYDiagnosisRequiredNotFoundException => new ProblemDetails { Status=404,Title="Kayıt bulunamadı.",Detail=exception.Message },
            SYYDiagnosisRequiredConflictException => new ProblemDetails { Status=409,Title="Tanım çakışması.",Detail=exception.Message },
            ICD10DefNotFoundException => new ProblemDetails{Status=404,Title="Kayıt bulunamadı.",Detail=exception.Message},
            ICD10DefConflictException => new ProblemDetails{Status=409,Title="ICD10 tanım çakışması.",Detail=exception.Message},
            ICD10DiagnosticNotFoundException => new ProblemDetails{Status=404,Title="Kayıt bulunamadı.",Detail=exception.Message},
            ICD10DiagnosticConflictException => new ProblemDetails{Status=409,Title="ICD10 istatistik çakışması.",Detail=exception.Message},
            SYYDiagnosisDefNotFoundException => new ProblemDetails{Status=404,Title="Kayıt bulunamadı.",Detail=exception.Message},
            SYYDiagnosisDefConflictException => new ProblemDetails{Status=409,Title="Tanım işlemi gerçekleştirilemedi.",Detail=exception.Message},
            SYYDiagnosisTypeDefNotFoundException => new ProblemDetails{Status=404,Title="Kayıt bulunamadı.",Detail=exception.Message},
            SYYDiagnosisTypeDefConflictException => new ProblemDetails{Status=409,Title="Tanım işlemi gerçekleştirilemedi.",Detail=exception.Message},
            SYYDiagnosisSliceDefNotFoundException => new ProblemDetails{Status=404,Title="Kayıt bulunamadı.",Detail=exception.Message},
            SYYDiagnosisSliceDefConflictException => new ProblemDetails{Status=409,Title="Tanım işlemi gerçekleştirilemedi.",Detail=exception.Message},
            AnnotationReasonDefNotFoundException => new ProblemDetails{Status=404,Title="Kayıt bulunamadı.",Detail=exception.Message},
            AnnotationReasonDefConflictException => new ProblemDetails{Status=409,Title="Tanım işlemi gerçekleştirilemedi.",Detail=exception.Message},
            PlaneTypeDefNotFoundException => new ProblemDetails{Status=404,Title="Kayıt bulunamadı.",Detail=exception.Message},
            PlaneTypeDefConflictException => new ProblemDetails{Status=409,Title="Tanım işlemi gerçekleştirilemedi.",Detail=exception.Message},
            FlyingMissionDefNotFoundException => new ProblemDetails{Status=404,Title="Kayıt bulunamadı.",Detail=exception.Message},
            FlyingMissionDefConflictException => new ProblemDetails{Status=409,Title="Tanım işlemi gerçekleştirilemedi.",Detail=exception.Message},
            ProcedureTypeNotFoundException => new ProblemDetails { Status=404, Title="Tetkik Türü bulunamadı.", Detail=exception.Message },
            ProcedureTypeConflictException => new ProblemDetails { Status=409, Title="Tetkik Türü işlemi gerçekleştirilemedi.", Detail=exception.Message },
            ProcedureNotFoundException => new ProblemDetails { Status=404, Title="Kayıt bulunamadı.", Detail=exception.Message },
            ProcedureConflictException => new ProblemDetails { Status=409, Title="Eşleştirme işlemi gerçekleştirilemedi.", Detail=exception.Message },
            ReasonForExamReportTypeNotFoundException => new ProblemDetails { Status=404, Title="Kayıt bulunamadı.", Detail=exception.Message },
            ReasonForExamReportTypeConflictException => new ProblemDetails { Status=409, Title="Eşleştirme işlemi gerçekleştirilemedi.", Detail=exception.Message },
            ReasonForExamUnusedHcAdmissionTypeNotFoundException => new ProblemDetails { Status=404, Title="Kayıt bulunamadı.", Detail=exception.Message },
            ReasonForExamUnusedHcAdmissionTypeConflictException => new ProblemDetails { Status=409, Title="Eşleştirme işlemi gerçekleştirilemedi.", Detail=exception.Message },
            ReasonForExamProcedureDefNotFoundException => new ProblemDetails { Status=404, Title="Kayıt bulunamadı.", Detail=exception.Message },
            ReasonForExamProcedureDefConflictException => new ProblemDetails { Status=409, Title="Eşleştirme işlemi gerçekleştirilemedi.", Detail=exception.Message },
            ReasonForExamBranchDefNotFoundException => new ProblemDetails { Status=404, Title="Kayıt bulunamadı.", Detail=exception.Message },
            ReasonForExamBranchDefConflictException => new ProblemDetails { Status=409, Title="Eşleştirme işlemi gerçekleştirilemedi.", Detail=exception.Message },
            HospitalNotFoundException =>
                new ProblemDetails { Status = StatusCodes.Status404NotFound,
                    Title = "Sağlık tesisi bulunamadı.", Detail = exception.Message },
            HospitalConflictException =>
                new ProblemDetails { Status = StatusCodes.Status409Conflict,
                    Title = "Sağlık tesisi işlemi gerçekleştirilemedi.", Detail = exception.Message },
            HospitalReportTypeNotFoundException =>
                new ProblemDetails { Status = StatusCodes.Status404NotFound,
                    Title = "Tesis rapor türü eşleştirmesi bulunamadı.", Detail = exception.Message },
            HospitalReportTypeConflictException =>
                new ProblemDetails { Status = StatusCodes.Status409Conflict,
                    Title = "Tesis rapor türü işlemi gerçekleştirilemedi.", Detail = exception.Message },
            ReportTypeNotFoundException =>
                new ProblemDetails { Status = StatusCodes.Status404NotFound,
                    Title = "Rapor Türü bulunamadı.", Detail = exception.Message },
            ReportTypeConflictException =>
                new ProblemDetails { Status = StatusCodes.Status409Conflict,
                    Title = "Rapor Türü işlemi gerçekleştirilemedi.", Detail = exception.Message },
            RankGroupNotFoundException =>
                new ProblemDetails { Status = StatusCodes.Status404NotFound,
                    Title = "Rütbe Grubu bulunamadı.", Detail = exception.Message },
            RankGroupConflictException =>
                new ProblemDetails { Status = StatusCodes.Status409Conflict,
                    Title = "Rütbe Grubu işlemi gerçekleştirilemedi.", Detail = exception.Message },
            RankNotFoundException =>
                new ProblemDetails { Status = StatusCodes.Status404NotFound,
                    Title = "Rütbe bulunamadı.", Detail = exception.Message },
            RankConflictException =>
                new ProblemDetails { Status = StatusCodes.Status409Conflict,
                    Title = "Rütbe işlemi gerçekleştirilemedi.", Detail = exception.Message },
            BranchNotFoundException =>
                new ProblemDetails { Status = StatusCodes.Status404NotFound,
                    Title = "Branş bulunamadı.", Detail = exception.Message },
            BranchConflictException =>
                new ProblemDetails { Status = StatusCodes.Status409Conflict,
                    Title = "Branş işlemi gerçekleştirilemedi.", Detail = exception.Message },
            ConnectedDecisionNotFoundException =>
                new ProblemDetails { Status = StatusCodes.Status404NotFound,
                    Title = "Bağlı kurul kararı bulunamadı.", Detail = exception.Message },
            ConnectedDecisionConflictException =>
                new ProblemDetails { Status = StatusCodes.Status409Conflict,
                    Title = "Bağlı kurul kararı işlemi gerçekleştirilemedi.", Detail = exception.Message },
            AdmissionTypeNotFoundException =>
                new ProblemDetails { Status = StatusCodes.Status404NotFound,
                    Title = "Başvuru grubu bulunamadı.", Detail = exception.Message },
            AdmissionTypeConflictException =>
                new ProblemDetails { Status = StatusCodes.Status409Conflict,
                    Title = "Başvuru grubu işlemi gerçekleştirilemedi.", Detail = exception.Message },
            MilitaryOfficeNotFoundException =>
                new ProblemDetails { Status = StatusCodes.Status404NotFound,
                    Title = "Askerlik şubesi bulunamadı.", Detail = exception.Message },
            MilitaryOfficeConflictException =>
                new ProblemDetails { Status = StatusCodes.Status409Conflict,
                    Title = "Askerlik şubesi işlemi gerçekleştirilemedi.", Detail = exception.Message },
            MilitarySchoolNotFoundException =>
                new ProblemDetails { Status = StatusCodes.Status404NotFound,
                    Title = "Askerî okul bulunamadı.", Detail = exception.Message },
            MilitarySchoolConflictException =>
                new ProblemDetails { Status = StatusCodes.Status409Conflict,
                    Title = "Askerî okul işlemi gerçekleştirilemedi.", Detail = exception.Message },
            CityNotFoundException =>
                new ProblemDetails { Status = StatusCodes.Status404NotFound,
                    Title = "Şehir bulunamadı.", Detail = exception.Message },
            CityConflictException =>
                new ProblemDetails { Status = StatusCodes.Status409Conflict,
                    Title = "Şehir işlemi gerçekleştirilemedi.", Detail = exception.Message },

            MilitaryUnitNotFoundException =>
                new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "Birlik bulunamadı.",
                    Detail = exception.Message
                },

            MilitaryUnitConflictException =>
                new ProblemDetails
                {
                    Status = StatusCodes.Status409Conflict,
                    Title = "Birlik işlemi gerçekleştirilemedi.",
                    Detail = exception.Message
                },

            InvalidDataException =>
                new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Geçersiz PDF dosyası.",
                    Detail = exception.Message
                },

            OutsideValidityPeriodException =>
                new ProblemDetails
                {
                    Status = StatusCodes.Status403Forbidden,
                    Title = "Hesap geçerlilik süresi dışında.",
                    Detail = exception.Message
                },

            OutsideWorkingHoursException =>
                new ProblemDetails
                {
                    Status = StatusCodes.Status403Forbidden,
                    Title = "Mesai dışı erişim engellendi.",
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
