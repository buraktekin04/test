using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Dispatches.Exceptions;
using KLMN.Application.Dispatches.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.Dispatches.Queries.GetDispatchFiles;
/// <summary>Sevke ait dosyalar yalnız izin kontrolünden geçmiş kullanıcıya listelenir.</summary>
public sealed class GetDispatchFilesQueryHandler(IKLMNDbContext db)
    :IRequestHandler<GetDispatchFilesQuery,IReadOnlyList<DispatchFileResponse>>
{
    public async Task<IReadOnlyList<DispatchFileResponse>> Handle(GetDispatchFilesQuery request,CancellationToken ct)
    {
        if(!await db.Dispatches.AnyAsync(x=>x.Id==request.DispatchId&&!x.IsDeleted,ct))
            throw new DispatchNotFoundException();
        return await db.DispatchFiles.AsNoTracking().Where(x=>x.DispatchId==request.DispatchId&&!x.IsDeleted)
            .OrderByDescending(x=>x.CreatedDate)
            .Select(x=>new DispatchFileResponse(x.Id,x.FileName,x.ContentType,x.FileSize,x.CreatedDate))
            .ToListAsync(ct);
    }
}
