using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Dispatches.Exceptions;
using KLMN.Application.Dispatches.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace KLMN.Application.Dispatches.Commands.CancelDispatch;
/// <summary>Yalnız yeni sevkler iptal edilir, geçmiş ve ekler korunur.</summary>
public sealed class CancelDispatchCommandHandler(IKLMNDbContext db):IRequestHandler<CancelDispatchCommand,DispatchMutationResponse>
{
    public async Task<DispatchMutationResponse> Handle(CancelDispatchCommand request,CancellationToken ct)
    {
        var item=await db.Dispatches.SingleOrDefaultAsync(x=>x.Id==request.Id&&!x.IsDeleted,ct)
            ??throw new DispatchNotFoundException();
        if(item.Version!=request.Version)throw new DbUpdateConcurrencyException("Sevk başka bir kullanıcı tarafından değiştirildi.");
        if(item.CurrentState!=1)throw new DispatchConflictException("Yalnız Yeni durumundaki sevkler iptal edilebilir.");
        item.CurrentState=4;
        await db.SaveChangesAsync(ct);
        return new DispatchMutationResponse(item.Id,item.DispatchNo,item.Version);
    }
}
