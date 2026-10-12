using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Common.Interfaces.Storage;
using KLMN.Application.Users.Agreements.Responses;
using KLMN.Application.Users.Exceptions;
using KLMN.Domain.Entities.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Users.Agreements.Commands.UploadUserAgreementPdf;

/// <summary>PDF'nin güvenli diske yazımıyla veritabanı metadata kaydını ilişkilendirir.</summary>
public sealed class UploadUserAgreementPdfCommandHandler(
    IKLMNDbContext dbContext,
    IUserAgreementPdfStorage storage,
    TimeProvider timeProvider)
    : IRequestHandler<UploadUserAgreementPdfCommand, UserAgreementPdfResponse>
{
    /// <summary>
    /// Kullanıcının mevcut olduğunu kontrol eder; PDF içeriğini diske yazar,
    /// yalnızca metadata kaydını PostgreSQL'e ekler.
    /// </summary>
    public async Task<UserAgreementPdfResponse> Handle(
        UploadUserAgreementPdfCommand request, CancellationToken cancellationToken)
    {
        // İzin verilen en büyük dosya uzunluğu sunucu appsettings'indeki değerdir.
        if (request.SizeBytes > storage.MaxFileSizeBytes)
            throw new InvalidDataException("PDF izin verilen maksimum boyutu aşıyor.");

        var exists = await dbContext.Users.IgnoreQueryFilters()
            .AnyAsync(u => u.Id == request.UserId && !u.IsDeleted, cancellationToken);
        if (!exists) throw new UserNotFoundException();

        // Fiziksel dosya yolu dışarıya verilmez; yalnızca güvenli relative key saklanır.
        var stored = await storage.SaveAsync(request.UserId, request.Content, cancellationToken);

        try
        {
            var document = new UserAgreementPdf
            {
                UserId = request.UserId,
                OriginalFileName = request.OriginalFileName,
                RelativePath = stored.RelativePath,
                SizeBytes = stored.SizeBytes,
                ContentType = "application/pdf"
            };

            await dbContext.UserAgreementPdfs.AddAsync(document, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            return new UserAgreementPdfResponse
            {
                Id = document.Id,
                UserId = document.UserId,
                FileName = document.OriginalFileName,
                SizeBytes = document.SizeBytes,
                CreatedDate = document.CreatedDate
            };
        }
        catch
        {
            // Metadata DB'ye yazılamazsa yetimsiz PDF dosyası bırakılmaz.
            await storage.DeleteAsync(stored.RelativePath, CancellationToken.None);
            throw;
        }
    }
}
