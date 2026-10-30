using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Dispatches.Exceptions;
using KLMN.Application.Dispatches.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Dispatches.Queries.GetDispatchPhoto;

/// <summary>Fotoğrafı yalnız sevk kimliğiyle sorgular; normal liste ve audit sorgularında fotoğraf bulunmaz.</summary>
public sealed class GetDispatchPhotoQueryHandler(IKLMNDbContext db):
    IRequestHandler<GetDispatchPhotoQuery,DispatchPhotoResponse>
{
    public async Task<DispatchPhotoResponse> Handle(GetDispatchPhotoQuery request,CancellationToken ct)
    {
        return await db.Dispatches.AsNoTracking()
            .Where(x=>x.Id==request.Id&&!x.IsDeleted&&x.Photo!=null)
            .Select(x=>new DispatchPhotoResponse(x.PhotoContentType!,x.Photo!))
            .SingleOrDefaultAsync(ct)??throw new DispatchNotFoundException();
    }
}
