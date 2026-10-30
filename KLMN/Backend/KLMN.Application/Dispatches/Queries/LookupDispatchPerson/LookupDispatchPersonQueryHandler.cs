using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Dispatches.Exceptions;
using KLMN.Application.Dispatches.Models;
using KLMN.Application.Dispatches.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Dispatches.Queries.LookupDispatchPerson;

/// <summary>Kimlik ve doğum tarihi beraber eşleşmeden kişisel bilgileri ifşa etmez.</summary>
public sealed class LookupDispatchPersonQueryHandler(IKLMNDbContext db)
    : IRequestHandler<LookupDispatchPersonQuery,DispatchPersonResponse?>
{
    public async Task<DispatchPersonResponse?> Handle(LookupDispatchPersonQuery request,CancellationToken ct)
    {
        var tc=request.TcKimlikNo?.Trim();
        var foreign=request.YabanciKimlikNo?.Trim();
        if (string.IsNullOrEmpty(tc)==string.IsNullOrEmpty(foreign)||request.BirthDate==default)
            throw new DispatchConflictException("Tek bir kimlik numarası ve doğum tarihi giriniz.");
        return await db.Persons.AsNoTracking().Where(x=>!x.IsDeleted && x.BirthDate==request.BirthDate &&
            (tc!=null ? x.TcKimlikNo==tc : x.YabanciKimlikNo==foreign))
            .Select(x=>new DispatchPersonResponse(new DispatchPersonInput{
                TcKimlikNo=x.TcKimlikNo,YabanciKimlikNo=x.YabanciKimlikNo,
                Name=x.Name,Surname=x.Surname,MotherName=x.MotherName,FatherName=x.FatherName,
                NationalityId=x.NationalityId,TownOfBirthId=x.TownOfBirthId,
                BirthDate=x.BirthDate,Gender=x.Gender,PhoneNumber=x.PhoneNumber,
                Address=x.Address,BirthPlaceText=x.BirthPlaceText,BirthPlaceCode=x.BirthPlaceCode
            })).SingleOrDefaultAsync(ct);
    }
}
