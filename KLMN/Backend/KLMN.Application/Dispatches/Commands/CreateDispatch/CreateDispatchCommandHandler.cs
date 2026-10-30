using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Dispatches.Commands.Common;
using KLMN.Application.Dispatches.Exceptions;
using KLMN.Application.Dispatches.Responses;
using KLMN.Domain.Entities.Admissions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Dispatches.Commands.CreateDispatch;

/// <summary>Tek SaveChanges ile kişi ve sevki kaydeder; eski MERNİS servisi kullanılmaz.</summary>
public sealed class CreateDispatchCommandHandler(IKLMNDbContext db) : IRequestHandler<CreateDispatchCommand,DispatchMutationResponse>
{
    public async Task<DispatchMutationResponse> Handle(CreateDispatchCommand request,CancellationToken ct)
    {
        await DispatchRules.ValidateAsync(db,request.Person,request.Dispatch,ct);
        var tc=request.Person.TcKimlikNo?.Trim();
        var foreign=request.Person.YabanciKimlikNo?.Trim();
        var person=await db.Persons.SingleOrDefaultAsync(x=>!x.IsDeleted &&
            (tc!=null ? x.TcKimlikNo==tc : x.YabanciKimlikNo==foreign),ct);
        if (person!=null && person.BirthDate!=request.Person.BirthDate)
            throw new DispatchConflictException("Bu kimlik numarasının kayıtlı doğum tarihi ile girilen tarih uyuşmuyor.");
        if (person==null)
        {
            person=new Person();
            await db.Persons.AddAsync(person,ct);
        }
        DispatchRules.AssignPerson(person,request.Person);
        var dispatch=new Dispatch{Person=person,CurrentState=1,AutomaticDispatch=false,DispatchDate=DateTime.UtcNow};
        DispatchRules.AssignDispatch(dispatch,request.Dispatch);
        await db.Dispatches.AddAsync(dispatch,ct);
        await db.SaveChangesAsync(ct);
        return new DispatchMutationResponse(dispatch.Id,dispatch.DispatchNo,dispatch.Version);
    }
}
