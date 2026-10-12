using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Users.Agreements.Responses;
using KLMN.Application.Users.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Users.Agreements.Queries.GetUserAgreementPdfs;

/// <summary>Kullanıcının silinmemiş PDF meta verilerini tarih sırasıyla listeler.</summary>
public sealed class GetUserAgreementPdfsQueryHandler(IKLMNDbContext dbContext)
    : IRequestHandler<GetUserAgreementPdfsQuery, IReadOnlyList<UserAgreementPdfResponse>>
{
    /// <summary>Dosya içeriğini veya fiziksel dizin bilgisini açığa çıkarmaz.</summary>
    public async Task<IReadOnlyList<UserAgreementPdfResponse>> Handle(
        GetUserAgreementPdfsQuery request, CancellationToken cancellationToken)
    {
        var userExists = await dbContext.Users.IgnoreQueryFilters()
            .AnyAsync(x => x.Id == request.UserId && !x.IsDeleted, cancellationToken);
        if (!userExists) throw new UserNotFoundException();

        return await dbContext.UserAgreementPdfs
            .AsNoTracking()
            .Where(x => x.UserId == request.UserId)
            .OrderByDescending(x => x.CreatedDate)
            .Select(x => new UserAgreementPdfResponse
            {
                Id = x.Id,
                UserId = x.UserId,
                FileName = x.OriginalFileName,
                SizeBytes = x.SizeBytes,
                CreatedDate = x.CreatedDate
            })
            .ToListAsync(cancellationToken);
    }
}
