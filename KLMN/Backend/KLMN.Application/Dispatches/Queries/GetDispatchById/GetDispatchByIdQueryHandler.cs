using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Dispatches.Exceptions;
using KLMN.Application.Dispatches.Models;
using KLMN.Application.Dispatches.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Dispatches.Queries.GetDispatchById;

/// <summary>Sevk ve kişi projeksiyonu QueryHandler içinde kalır; Response statik Expression içermez.</summary>
public sealed class GetDispatchByIdQueryHandler(IKLMNDbContext db)
    : IRequestHandler<GetDispatchByIdQuery,DispatchResponse>
{
    public async Task<DispatchResponse> Handle(GetDispatchByIdQuery request,CancellationToken ct)
    {
        return await db.Dispatches.AsNoTracking().Where(x=>x.Id==request.Id&&!x.IsDeleted)
            .Select(x=>new DispatchResponse(
                x.Id,x.DispatchNo,x.DispatchDate,x.CurrentState,x.IsActive,x.Version,
                new DispatchPersonInput{
                    TcKimlikNo=x.Person.TcKimlikNo,YabanciKimlikNo=x.Person.YabanciKimlikNo,
                    Name=x.Person.Name,Surname=x.Person.Surname,MotherName=x.Person.MotherName,
                    FatherName=x.Person.FatherName,NationalityId=x.Person.NationalityId,
                    TownOfBirthId=x.Person.TownOfBirthId,BirthDate=x.Person.BirthDate,
                    Gender=x.Person.Gender,PhoneNumber=x.Person.PhoneNumber,Address=x.Person.Address,
                    BirthPlaceText=x.Person.BirthPlaceText,BirthPlaceCode=x.Person.BirthPlaceCode
                },
                new DispatchDetailsInput{
                    NthAdmission=x.NthAdmission,DispatchDocumentDate=x.DispatchDocumentDate,
                    DispatchDocumentNo=x.DispatchDocumentNo,ReasonForExaminationDefId=x.ReasonForExaminationDefId,
                    DispatchScope=x.DispatchScope,DispatchScopeCityId=x.DispatchScopeCityId,
                    DispatchScopeHospitalId=x.DispatchScopeHospitalId,HcAdmissionTypeDefId=x.HcAdmissionTypeDefId,
                    ForcesCommandDefId=x.ForcesCommandDefId,RankDefId=x.RankDefId,
                    MilitaryClassDefId=x.MilitaryClassDefId,MilitaryUnitDefId=x.MilitaryUnitDefId,
                    MilitarySchoolDefId=x.MilitarySchoolDefId,
                    SenderMilitaryUnitDefId=x.SenderMilitaryUnitDefId,
                    SenderMilitaryOfficeDefId=x.SenderMilitaryOfficeDefId,SenderChairOther=x.SenderChairOther,
                    MilitaryOfficeDefId=x.MilitaryOfficeDefId,FlyingMissionDefId=x.FlyingMissionDefId,
                    PlaneTypeDefId=x.PlaneTypeDefId,RegistryNumber=x.RegistryNumber,
                    LastNominationDate=x.LastNominationDate,MilitaryUnitDescription=x.MilitaryUnitDescription,
                    PurchaseId=x.PurchaseId,Description=x.Description
                })).SingleOrDefaultAsync(ct)??throw new DispatchNotFoundException();
    }
}
