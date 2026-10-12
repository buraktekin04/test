using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Common.Interfaces.Storage;
using KLMN.Application.Users.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace KLMN.Application.Users.Agreements.Commands.DeleteUserAgreementPdf;

/// <summary>PDF metadata kaydını soft delete eder ve private disk dosyasını kaldırır.</summary>
public sealed class DeleteUserAgreementPdfCommandHandler(
    IKLMNDbContext dbContext,
    IUserAgreementPdfStorage storage,
    ILogger<DeleteUserAgreementPdfCommandHandler> logger)
    : IRequestHandler<DeleteUserAgreementPdfCommand>
{
    /// <summary>Veritabanı kaydını silinmiş işaretledikten sonra dosyayı temizler.</summary>
    public async Task Handle(DeleteUserAgreementPdfCommand request, CancellationToken cancellationToken)
    {
        var pdf = await dbContext.UserAgreementPdfs
            .SingleOrDefaultAsync(x =>
                x.Id == request.DocumentId && x.UserId == request.UserId,
                cancellationToken);

        if (pdf is null) throw new UserNotFoundException();

        // Audit interceptor Remove -> IsDeleted=true davranışını uygular.
        dbContext.UserAgreementPdfs.Remove(pdf);
        await dbContext.SaveChangesAsync(cancellationToken);

        try
        {
            await storage.DeleteAsync(pdf.RelativePath, cancellationToken);
        }
        catch (IOException ex)
        {
            // Başarılı DB soft delete'ini geriye döndürmeyiz. Yetim dosyanın
            // temizlenebilmesi için güvenli metadata olmadan teknik uyarı bırakırız.
            logger.LogWarning(ex, "PDF metadata silindi fakat fiziksel dosya kaldı. DocumentId: {DocumentId}", pdf.Id);
        }
    }
}
