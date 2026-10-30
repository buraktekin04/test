using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Dispatches.Exceptions;
using KLMN.Application.Dispatches.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.Dispatches.Queries.GetDispatchFile;
/// <summary>Dosya aidiyetini doğrulayıp yalnız ilgili sevkin eki olarak döndürür.</summary>
public sealed class GetDispatchFileQueryHandler(IKLMNDbContext db):IRequestHandler<GetDispatchFileQuery,DispatchFileDataResponse>
{
    public async Task<DispatchFileDataResponse> Handle(GetDispatchFileQuery request,CancellationToken ct)
    {
        return await db.DispatchFiles.AsNoTracking()
            .Where(x=>x.Id==request.FileId&&x.DispatchId==request.DispatchId&&!x.IsDeleted&&!x.Dispatch.IsDeleted)
            .Select(x=>new DispatchFileDataResponse(x.FileName,x.ContentType,x.DataContent))
            .SingleOrDefaultAsync(ct)??throw new DispatchNotFoundException();
    }
}
