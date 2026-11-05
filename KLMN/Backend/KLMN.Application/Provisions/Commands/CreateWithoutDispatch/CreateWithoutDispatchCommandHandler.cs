using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Dispatches.Commands.Common;
using KLMN.Application.Dispatches.Exceptions;
using KLMN.Application.Provisions.Exceptions;
using KLMN.Application.Provisions.Responses;
using KLMN.Domain.Entities.Admissions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Provisions.Commands.CreateWithoutDispatch;

/// <summary>
/// Eski modelin NOT NULL Provision.Dispatch ilişkisi için aynı transaction içinde
/// sentetik sevk + kişi + provizyon + ilk durum oluşturur; dış servisi taklit etmez.
/// </summary>
public sealed class CreateWithoutDispatchCommandHandler(IKLMNDbContext db)
    : IRequestHandler<CreateWithoutDispatchCommand,ProvisionMutationResponse>
{
    public async Task<ProvisionMutationResponse> Handle(CreateWithoutDispatchCommand request,CancellationToken ct)
    {
        var hospital=await db.HospitalDefs.AsNoTracking().SingleOrDefaultAsync(
            x=>x.Id==request.HospitalId&&x.IsActive&&!x.IsDeleted,ct)
            ??throw new ProvisionConflictException("Kabul yapan hastane geçersiz.");

        // Sevksiz provizyonda UI kapsam seçtirmez; kabul hastanesi belirli tesis kapsamıdır.
        // Eski Dispatch kaydı mevcut değildir, ancak Provision.Dispatch FK'si korunur.
        request.Dispatch.DispatchScope=3;
        request.Dispatch.DispatchScopeHospitalId=hospital.Id;
        request.Dispatch.DispatchScopeCityId=null;
        await DispatchRules.ValidateAsync(db,request.Person,request.Dispatch,ct);
        var tc=request.Person.TcKimlikNo?.Trim();
        var foreign=request.Person.YabanciKimlikNo?.Trim();
        var person=await db.Persons.SingleOrDefaultAsync(x=>!x.IsDeleted &&
            (tc!=null ? x.TcKimlikNo==tc : x.YabanciKimlikNo==foreign),ct);
        if(person!=null && person.BirthDate!=request.Person.BirthDate)
            throw new DispatchConflictException("Kayıtlı doğum tarihi kimlikle uyuşmuyor.");
        if(person is null)
        {
            person=new Person();
            await db.Persons.AddAsync(person,ct);
        }
        DispatchRules.AssignPerson(person,request.Person);
        var dispatch=new Dispatch
        {
            Person=person,CurrentState=2,AutomaticDispatch=true,
            DispatchDate=DateTime.UtcNow
        };
        DispatchRules.AssignDispatch(dispatch,request.Dispatch);
        await db.Dispatches.AddAsync(dispatch,ct);
        byte[]? photo=null;
        if(!string.IsNullOrWhiteSpace(request.PhotoBase64))
        {
            if(request.PhotoBase64.Length>2_800_000)
                throw new ProvisionConflictException("Fotoğraf en fazla 2 MiB olabilir.");
            try { photo=Convert.FromBase64String(request.PhotoBase64); }
            catch(FormatException) { throw new ProvisionConflictException("Fotoğraf Base64 biçiminde değil."); }
            if(photo.Length==0 || photo.Length>2*1024*1024)
                throw new ProvisionConflictException("Fotoğraf boyutu geçersiz.");
            var jpeg=photo.Length>=3&&photo[0]==0xFF&&photo[1]==0xD8&&photo[2]==0xFF;
            var png=photo.Length>=8&&photo[0]==0x89&&photo[1]==0x50&&photo[2]==0x4E&&photo[3]==0x47
                &&photo[4]==0x0D&&photo[5]==0x0A&&photo[6]==0x1A&&photo[7]==0x0A;
            if(!jpeg&&!png) throw new ProvisionConflictException("Yalnız JPEG veya PNG fotoğraf kabul edilir.");
        }
        var provision=new Provision
        {
            Dispatch=dispatch,DispatchHospitalId=hospital.Id,
            Photo=photo,
            WithoutDispatch=1,CurrentState=1,ProvisionDate=DateTime.UtcNow,
            ObjectionableReportId=request.ObjectionableReportId
        };
        await db.Provisions.AddAsync(provision,ct);
        await db.ProvisionStates.AddAsync(new ProvisionState
        {
            Provision=provision,State=1
        },ct);
        await db.SaveChangesAsync(ct);
        return new ProvisionMutationResponse(provision.Id,provision.ProvisionNo,dispatch.Id,provision.Version);
    }
}
