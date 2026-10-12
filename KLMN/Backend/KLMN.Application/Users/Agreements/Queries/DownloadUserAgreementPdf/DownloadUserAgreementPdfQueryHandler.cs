using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Common.Interfaces.Storage;
using KLMN.Application.Users.Agreements.Responses;
using KLMN.Application.Users.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Users.Agreements.Queries.DownloadUserAgreementPdf;

/// <summary>Veritabanındaki kullanıcıya bağlı güvenli anahtarla PDF akışını açar.</summary>
public sealed class DownloadUserAgreementPdfQueryHandler(
    IKLMNDbContext dbContext, IUserAgreementPdfStorage storage)
    : IRequestHandler<DownloadUserAgreementPdfQuery, UserAgreementPdfDownload>
{
    /// <summary>PDF'nin doğru kullanıcıya bağlı olmasını doğrulayıp stream döndürür.</summary>
    public async Task<UserAgreementPdfDownload> Handle(
        DownloadUserAgreementPdfQuery request, CancellationToken cancellationToken)
    {
        // Soft delete edilmiş kullanıcıların belge içerikleri de API'den indirilemez.
        var activeUser = await dbContext.Users.IgnoreQueryFilters()
            .AnyAsync(x => x.Id == request.UserId && !x.IsDeleted, cancellationToken);
        if (!activeUser) throw new UserNotFoundException();

        var pdf = await dbContext.UserAgreementPdfs
            .AsNoTracking()
            .Where(x => x.UserId == request.UserId && x.Id == request.DocumentId)
            .Select(x => new
            {
                x.RelativePath,
                x.OriginalFileName
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (pdf is null) throw new UserNotFoundException();

        try
        {
            var stream = await storage.OpenReadAsync(pdf.RelativePath, cancellationToken);
            return new UserAgreementPdfDownload(stream, pdf.OriginalFileName);
        }
        catch (FileNotFoundException)
        {
            throw new UserNotFoundException();
        }
        catch (DirectoryNotFoundException)
        {
            throw new UserNotFoundException();
        }
    }
}
