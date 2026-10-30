using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Dispatches.Exceptions;
using KLMN.Application.Dispatches.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Dispatches.Commands.SetDispatchPhoto;

/// <summary>Yeni sevke fotoğraf kaydeder; MIME ve magic bytes uyuşmazlığını reddeder.</summary>
public sealed class SetDispatchPhotoCommandHandler(IKLMNDbContext db):
    IRequestHandler<SetDispatchPhotoCommand,DispatchMutationResponse>
{
    public async Task<DispatchMutationResponse> Handle(SetDispatchPhotoCommand request,CancellationToken ct)
    {
        var item=await db.Dispatches.SingleOrDefaultAsync(x=>x.Id==request.Id&&!x.IsDeleted,ct)
            ??throw new DispatchNotFoundException();
        if(item.CurrentState!=1)
            throw new DispatchConflictException("Yalnız Yeni durumundaki sevkin fotoğrafı değiştirilebilir.");
        var bytes=request.Content;
        if(bytes.Length is 0 or >2097152)
            throw new DispatchConflictException("Fotoğraf en fazla 2 MiB olabilir.");
        var jpeg=request.ContentType=="image/jpeg"&&bytes.Length>=4&&
            bytes[0]==0xff&&bytes[1]==0xd8&&bytes[^2]==0xff&&bytes[^1]==0xd9;
        var png=request.ContentType=="image/png"&&bytes.Length>=8&&
            bytes[0]==0x89&&bytes[1]==0x50&&bytes[2]==0x4e&&bytes[3]==0x47&&
            bytes[4]==0x0d&&bytes[5]==0x0a&&bytes[6]==0x1a&&bytes[7]==0x0a;
        if(!jpeg&&!png)
            throw new DispatchConflictException("Fotoğraf içeriği JPEG/PNG biçimiyle eşleşmiyor.");
        item.Photo=bytes;
        item.PhotoContentType=request.ContentType;
        await db.SaveChangesAsync(ct);
        return new DispatchMutationResponse(item.Id,item.DispatchNo,item.Version);
    }
}
