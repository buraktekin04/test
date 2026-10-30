using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Dispatches.Commands.Common;
using KLMN.Application.Dispatches.Exceptions;
using KLMN.Application.Dispatches.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Dispatches.Commands.UpdateDispatch;

/// <summary>Yalnız Yeni durumundaki sevk güncellenebilir; başka kişiye kimlik kaydırılmaz.</summary>
public sealed class UpdateDispatchCommandHandler(IKLMNDbContext db) : IRequestHandler<UpdateDispatchCommand,DispatchMutationResponse>
{
    public async Task<DispatchMutationResponse> Handle(UpdateDispatchCommand request,CancellationToken ct)
    {
        var current=await db.Dispatches.Include(x=>x.Person)
            .SingleOrDefaultAsync(x=>x.Id==request.Id&&!x.IsDeleted,ct)
            ??throw new DispatchNotFoundException();
        if (current.Version!=request.Version)
            throw new DbUpdateConcurrencyException("Sevk başka bir kullanıcı tarafından değiştirildi. Sayfayı yenileyiniz.");
        if (current.CurrentState!=1)
            throw new DispatchConflictException("Yalnız Yeni durumundaki sevkler düzenlenebilir.");
        await DispatchRules.ValidateAsync(db,request.Person,request.Dispatch,ct);
        if (current.Person.TcKimlikNo!=request.Person.TcKimlikNo?.Trim() ||
            current.Person.YabanciKimlikNo!=request.Person.YabanciKimlikNo?.Trim() ||
            current.Person.BirthDate!=request.Person.BirthDate)
            throw new DispatchConflictException("Sevk kişisinin kimlik bilgileri değiştirilemez. Yeni kişi için yeni sevk açınız.");
        DispatchRules.AssignPerson(current.Person,request.Person);
        DispatchRules.AssignDispatch(current,request.Dispatch);
        await db.SaveChangesAsync(ct);
        return new DispatchMutationResponse(current.Id,current.DispatchNo,current.Version);
    }
}
