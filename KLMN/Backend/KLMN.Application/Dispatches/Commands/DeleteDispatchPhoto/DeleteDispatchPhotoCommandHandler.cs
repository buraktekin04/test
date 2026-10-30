using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Dispatches.Exceptions;
using KLMN.Application.Dispatches.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Dispatches.Commands.DeleteDispatchPhoto;

/// <summary>Fotoğrafı sevkin xmin sürümüyle güvenle kaldırır; sevk durumu ve belgeler korunur.</summary>
public sealed class DeleteDispatchPhotoCommandHandler(IKLMNDbContext db):
    IRequestHandler<DeleteDispatchPhotoCommand,DispatchMutationResponse>
{
    public async Task<DispatchMutationResponse> Handle(DeleteDispatchPhotoCommand request,CancellationToken ct)
    {
        var item=await db.Dispatches.SingleOrDefaultAsync(x=>x.Id==request.Id&&!x.IsDeleted,ct)
            ??throw new DispatchNotFoundException();
        if(item.Version!=request.Version)
            throw new DbUpdateConcurrencyException("Sevk değiştirildi; güncel kayıt tekrar açılmalıdır.");
        if(item.CurrentState!=1)
            throw new DispatchConflictException("Yalnız Yeni durumundaki sevkten fotoğraf kaldırılabilir.");
        item.Photo=null;
        item.PhotoContentType=null;
        await db.SaveChangesAsync(ct);
        return new DispatchMutationResponse(item.Id,item.DispatchNo,item.Version);
    }
}
