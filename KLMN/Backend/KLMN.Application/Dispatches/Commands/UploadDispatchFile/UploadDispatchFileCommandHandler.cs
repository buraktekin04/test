using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Dispatches.Exceptions;
using KLMN.Application.Dispatches.Responses;
using KLMN.Domain.Entities.Admissions;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.Dispatches.Commands.UploadDispatchFile;
/// <summary>Geçerli Yeni sevke izinli belge ekler; içerik 5 MiB sınırı ve dosya imzasıyla kontrol edilir.</summary>
public sealed class UploadDispatchFileCommandHandler(IKLMNDbContext db)
    : IRequestHandler<UploadDispatchFileCommand,DispatchFileResponse>
{
    public async Task<DispatchFileResponse> Handle(UploadDispatchFileCommand request,CancellationToken ct)
    {
        var dispatch=await db.Dispatches.AsNoTracking().FirstOrDefaultAsync(x=>x.Id==request.DispatchId&&!x.IsDeleted,ct)
            ??throw new DispatchNotFoundException();
        if(dispatch.CurrentState!=1)throw new DispatchConflictException("Yalnız Yeni durumundaki sevke dosya eklenebilir.");
        if(request.Content.Length is 0 or > 5242880)throw new DispatchConflictException("Dosya 1 byte ile 5 MiB arasında olmalıdır.");
        var signature=request.Content;
        var mime=request.ContentType.ToLowerInvariant();
        var isPdf=mime=="application/pdf"&&signature.Length>=5&&signature[0]==37&&signature[1]==80&&signature[2]==68&&signature[3]==70&&signature[4]==45;
        var isPng=mime=="image/png"&&signature.Length>=8&&signature[0]==137&&signature[1]==80&&signature[2]==78&&signature[3]==71;
        var isJpeg=mime=="image/jpeg"&&signature.Length>=3&&signature[0]==255&&signature[1]==216&&signature[2]==255;
        if(!isPdf&&!isPng&&!isJpeg)throw new DispatchConflictException("Yalnız geçerli PDF, PNG veya JPEG dosyaları kabul edilir.");
        var safeName=Path.GetFileName(request.FileName.Replace('\\','/')).Trim();
        if(safeName.Length is 0 or > 180)throw new DispatchConflictException("Dosya adı geçersiz.");
        var item=new DispatchFile{DispatchId=dispatch.Id,FileName=safeName,
            ContentType=mime,DataContent=request.Content,FileSize=request.Content.Length};
        await db.DispatchFiles.AddAsync(item,ct);
        await db.SaveChangesAsync(ct);
        return new DispatchFileResponse(item.Id,item.FileName,item.ContentType,item.FileSize,item.CreatedDate);
    }
}
