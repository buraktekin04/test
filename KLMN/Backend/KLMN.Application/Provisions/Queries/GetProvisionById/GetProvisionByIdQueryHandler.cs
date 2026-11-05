using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Dispatches.Models;
using KLMN.Application.Provisions.Exceptions;
using KLMN.Application.Provisions.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Provisions.Queries.GetProvisionById;

/// <summary>Provizyon detayı ve eski sevk/kişi GUID ilişkileri korunur.</summary>
public sealed class GetProvisionByIdQueryHandler(IKLMNDbContext db)
    : IRequestHandler<GetProvisionByIdQuery,ProvisionDetailResponse>
{
    public async Task<ProvisionDetailResponse> Handle(GetProvisionByIdQuery request,CancellationToken ct)
    {
        var item=await db.Provisions.AsNoTracking()
            .Include(x=>x.Dispatch).ThenInclude(x=>x.Person)
            .SingleOrDefaultAsync(x=>x.Id==request.Id&&!x.IsDeleted,ct)
            ??throw new ProvisionNotFoundException();
        var d=item.Dispatch;
        var p=d.Person;
        var states=await db.ProvisionStates.AsNoTracking()
            .Where(x=>x.ProvisionId==item.Id&&!x.IsDeleted)
            .OrderBy(x=>x.CreatedDate)
            .Select(x=>new ProvisionStateResponse(x.Id,x.State,x.CreatedDate,x.CreatedBy))
            .ToListAsync(ct);
        return new ProvisionDetailResponse(
            item.Id,item.ProvisionNo,item.DispatchId,d.DispatchNo,item.ProvisionDate,
            item.DispatchHospitalId,item.CurrentState,item.WithoutDispatch,
            item.ObjectionableReportId,item.Description,item.Version,
            new DispatchPersonInput {
                TcKimlikNo=p.TcKimlikNo,YabanciKimlikNo=p.YabanciKimlikNo,
                Name=p.Name,Surname=p.Surname,MotherName=p.MotherName,
                FatherName=p.FatherName,NationalityId=p.NationalityId,
                TownOfBirthId=p.TownOfBirthId,BirthDate=p.BirthDate,
                Gender=p.Gender,PhoneNumber=p.PhoneNumber,Address=p.Address,
                BirthPlaceText=p.BirthPlaceText,BirthPlaceCode=p.BirthPlaceCode
            },
            new DispatchDetailsInput {
                NthAdmission=d.NthAdmission,DispatchDocumentDate=d.DispatchDocumentDate,
                DispatchDocumentNo=d.DispatchDocumentNo,ReasonForExaminationDefId=d.ReasonForExaminationDefId,
                DispatchScope=d.DispatchScope,DispatchScopeCityId=d.DispatchScopeCityId,
                DispatchScopeHospitalId=d.DispatchScopeHospitalId,
                HcAdmissionTypeDefId=d.HcAdmissionTypeDefId,ForcesCommandDefId=d.ForcesCommandDefId,
                RankDefId=d.RankDefId,MilitaryClassDefId=d.MilitaryClassDefId,
                MilitaryUnitDefId=d.MilitaryUnitDefId,MilitarySchoolDefId=d.MilitarySchoolDefId,
                SenderMilitaryUnitDefId=d.SenderMilitaryUnitDefId,
                SenderMilitaryOfficeDefId=d.SenderMilitaryOfficeDefId,
                SenderChairOther=d.SenderChairOther,MilitaryOfficeDefId=d.MilitaryOfficeDefId,
                FlyingMissionDefId=d.FlyingMissionDefId,PlaneTypeDefId=d.PlaneTypeDefId,
                RegistryNumber=d.RegistryNumber,LastNominationDate=d.LastNominationDate,
                MilitaryUnitDescription=d.MilitaryUnitDescription,PurchaseId=d.PurchaseId,
                Description=d.Description
            },states);
    }
}
