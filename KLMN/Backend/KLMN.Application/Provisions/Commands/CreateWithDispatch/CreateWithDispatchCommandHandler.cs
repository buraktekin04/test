using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Provisions.Exceptions;
using KLMN.Application.Provisions.Responses;
using KLMN.Domain.Entities.Admissions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Provisions.Commands.CreateWithDispatch;

/// <summary>Var olan sevk ve hastanenin GUID'sini değiştirmeden provizyona bağlar.</summary>
public sealed class CreateWithDispatchCommandHandler(IKLMNDbContext db)
    : IRequestHandler<CreateWithDispatchCommand,ProvisionMutationResponse>
{
    public async Task<ProvisionMutationResponse> Handle(CreateWithDispatchCommand request,CancellationToken ct)
    {
        var dispatch=await db.Dispatches.SingleOrDefaultAsync(
            x=>!x.IsDeleted && (request.DispatchId.HasValue
                ? x.Id==request.DispatchId.Value
                : x.DispatchNo==request.DispatchNo),ct)
            ??throw new ProvisionConflictException("Sevk kaydı bulunamadı.");
        if(dispatch.CurrentState!=1)
            throw new ProvisionConflictException("Yalnız Yeni durumundaki sevklere provizyon alınabilir.");
        if(await db.Provisions.AnyAsync(x=>x.DispatchId==dispatch.Id&&!x.IsDeleted,ct))
            throw new ProvisionConflictException("Sevke daha önce provizyon alınmış.");
        var hospital=await db.HospitalDefs.AsNoTracking().SingleOrDefaultAsync(
            x=>x.Id==request.HospitalId&&x.IsActive&&!x.IsDeleted,ct)
            ??throw new ProvisionConflictException("Kabul yapan hastane geçersiz.");
        if(dispatch.DispatchScope==3 && dispatch.DispatchScopeHospitalId!=hospital.Id)
            throw new ProvisionConflictException("Sevk yalnız belirtilen sağlık tesisinde geçerlidir.");
        if(dispatch.DispatchScope==2 && dispatch.DispatchScopeCityId!=hospital.CityId)
            throw new ProvisionConflictException("Kabul yapan hastane sevk kapsamı şehrinde değildir.");
        var provision=new Provision
        {
            DispatchId=dispatch.Id,DispatchHospitalId=hospital.Id,
            CurrentState=1,ProvisionDate=DateTime.UtcNow,WithoutDispatch=null
        };
        await db.Provisions.AddAsync(provision,ct);
        await db.ProvisionStates.AddAsync(new ProvisionState
        {
            Provision=provision,State=1
        },ct);
        dispatch.CurrentState=2;
        await db.SaveChangesAsync(ct);
        return new ProvisionMutationResponse(provision.Id,provision.ProvisionNo,dispatch.Id,provision.Version);
    }
}
