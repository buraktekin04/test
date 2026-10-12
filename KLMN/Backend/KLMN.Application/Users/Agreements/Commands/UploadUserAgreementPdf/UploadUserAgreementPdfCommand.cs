using KLMN.Application.Users.Agreements.Responses;
using MediatR;

namespace KLMN.Application.Users.Agreements.Commands.UploadUserAgreementPdf;

/// <summary>
/// Kullanıcıya ait PDF içeriğini sadece işlem anında Stream olarak alır.
/// Dosyanın binary içeriği Domain entity veya DB alanına kopyalanmaz.
/// </summary>
/// <param name="UserId">PDF'nin atanacağı kullanıcı kimliğidir.</param>
/// <param name="OriginalFileName">Kullanıcının yüklediği görünen PDF dosya adıdır.</param>
/// <param name="SizeBytes">HTTP multipart formunda bildirilen dosya uzunluğudur.</param>
/// <param name="Content">Dosya içeriğinin okunacağı stream nesnesidir.</param>
public sealed record UploadUserAgreementPdfCommand(
    Guid UserId, string OriginalFileName, long SizeBytes, Stream Content)
    : IRequest<UserAgreementPdfResponse>;
