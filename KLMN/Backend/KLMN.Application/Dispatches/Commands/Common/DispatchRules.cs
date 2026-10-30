using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Dispatches.Exceptions;
using KLMN.Application.Dispatches.Models;
using KLMN.Domain.Entities.Admissions;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Application.Dispatches.Commands.Common;

/// <summary>Sevk kayıtlarını mevcut tanım kayıtları, başvuru grubunun dinamik zorunlulukları ve kişi kimliği ile doğrular.</summary>
internal static class DispatchRules
{
    public static async Task ValidateAsync(IKLMNDbContext db,DispatchPersonInput p,DispatchDetailsInput d,CancellationToken ct)
    {
        var tc=p.TcKimlikNo?.Trim();
        var foreign=p.YabanciKimlikNo?.Trim();
        if (string.IsNullOrEmpty(tc)==string.IsNullOrEmpty(foreign))
            throw new DispatchConflictException("T.C. veya yabancı kimlik numarasından yalnızca biri girilmelidir.");
        if (tc is not null && (tc.Length!=11 || tc[0]=='0' || tc.Any(x=>!char.IsAsciiDigit(x))))
            throw new DispatchConflictException("T.C. kimlik numarası 11 haneli ve sayısal olmalıdır.");
        if (foreign is not null && (foreign.Length<11 || foreign.Length>20 || foreign.Any(x=>!char.IsAsciiDigit(x))))
            throw new DispatchConflictException("Yabancı kimlik numarası 11-20 haneli ve sayısal olmalıdır.");
        if (p.BirthDate==default || p.BirthDate>DateOnly.FromDateTime(DateTime.UtcNow))
            throw new DispatchConflictException("Geçerli bir doğum tarihi giriniz.");
        if (string.IsNullOrWhiteSpace(p.Name)||string.IsNullOrWhiteSpace(p.Surname))
            throw new DispatchConflictException("Ad ve soyad zorunludur.");
        if (p.Name.Length>64||p.Surname.Length>64||p.MotherName?.Length>64||p.FatherName?.Length>64||
            p.PhoneNumber?.Length>20||p.Address?.Length>512||p.BirthPlaceText?.Length>500||
            p.BirthPlaceCode?.Length>250)
            throw new DispatchConflictException("Kişi bilgilerinde izin verilen alan uzunluğu aşıldı.");
        if (d.DispatchScope is <1 or >3)
            throw new DispatchConflictException("Sevk kapsamı geçersizdir.");
        if (d.DispatchScope==2 && !d.DispatchScopeCityId.HasValue)
            throw new DispatchConflictException("Şehir kapsamı için sevk kapsamı şehri seçilmelidir.");
        if (d.DispatchScope==3 && !d.DispatchScopeHospitalId.HasValue)
            throw new DispatchConflictException("Sağlık tesisi kapsamı için sağlık tesisi seçilmelidir.");
        if ((d.DispatchScope!=2 && d.DispatchScopeCityId.HasValue)||
            (d.DispatchScope!=3 && d.DispatchScopeHospitalId.HasValue))
            throw new DispatchConflictException("Sevk kapsamıyla ilgisiz şehir/hastane seçimi temizlenmelidir.");
        if (d.SenderMilitaryUnitDefId.HasValue && d.SenderMilitaryOfficeDefId.HasValue)
            throw new DispatchConflictException("Gönderen makamda birlik veya askerlik şubesinden yalnızca biri seçilebilir.");
        if (d.DispatchDocumentNo?.Length>50||d.RegistryNumber?.Length>50||
            d.MilitaryUnitDescription?.Length>1000||d.SenderChairOther?.Length>1000||d.Description?.Length>500)
            throw new DispatchConflictException("Sevk bilgilerinde izin verilen alan uzunluğu aşıldı.");
        if (d.NthAdmission<1 || d.PurchaseId<0)
            throw new DispatchConflictException("Kaçıncı işlem pozitif, alım ID negatif olmayan değer olmalıdır.");
        var group=await db.HcAdmissionTypeDefs.AsNoTracking()
            .FirstOrDefaultAsync(x=>x.Id==d.HcAdmissionTypeDefId&&x.IsActive,ct)
            ??throw new DispatchConflictException("Aktif bir başvuru grubu seçiniz.");
        if (!await db.ReasonForExaminationDefs.AnyAsync(x=>x.Id==d.ReasonForExaminationDefId&&x.IsActive,ct))
            throw new DispatchConflictException("Aktif bir muayene sebebi seçiniz.");
        if (group.NthAdmissionRequired && !d.NthAdmission.HasValue ||
            group.DispatchDocumentDateRequired && !d.DispatchDocumentDate.HasValue ||
            group.DispatchDocumentNoRequired && string.IsNullOrWhiteSpace(d.DispatchDocumentNo) ||
            group.ForcesCommandRequired && !d.ForcesCommandDefId.HasValue ||
            group.RankRequired && !d.RankDefId.HasValue ||
            group.MilitaryClassRequired && !d.MilitaryClassDefId.HasValue ||
            group.MilitaryUnitRequired && !d.MilitaryUnitDefId.HasValue ||
            group.MilitaryOfficeRequired && !d.MilitaryOfficeDefId.HasValue ||
            group.MilitarySchoolRequired && !d.MilitarySchoolDefId.HasValue ||
            group.RegistryNumberRequired && string.IsNullOrWhiteSpace(d.RegistryNumber) ||
            group.LastNominationDateRequired && !d.LastNominationDate.HasValue ||
            group.SenderChairRequired && !d.SenderMilitaryUnitDefId.HasValue && !d.SenderMilitaryOfficeDefId.HasValue)
            throw new DispatchConflictException("Başvuru grubunun gerektirdiği sevk/askerî alanlardan biri boş.");
        // FK'ler hem aktif olmalıdır hem de DB düzeyinde Restrict ile korunur.
        if (p.NationalityId.HasValue && !await db.CountryDefs.AnyAsync(x=>x.Id==p.NationalityId && x.IsActive,ct)) throw new DispatchConflictException("Uyruğu geçersiz.");
        if (p.TownOfBirthId.HasValue && !await db.TownDefs.AnyAsync(x=>x.Id==p.TownOfBirthId && x.IsActive,ct)) throw new DispatchConflictException("Doğum yeri geçersiz.");
        if (d.DispatchScopeCityId.HasValue && !await db.CityDefs.AnyAsync(x=>x.Id==d.DispatchScopeCityId && x.IsActive,ct)) throw new DispatchConflictException("Sevk kapsamı şehri geçersiz.");
        if (d.DispatchScopeHospitalId.HasValue && !await db.HospitalDefs.AnyAsync(x=>x.Id==d.DispatchScopeHospitalId && x.IsActive,ct)) throw new DispatchConflictException("Sevk kapsamı hastanesi geçersiz.");
        if (d.ForcesCommandDefId.HasValue && !await db.ForcesCommandDefs.AnyAsync(x=>x.Id==d.ForcesCommandDefId && x.IsActive,ct)) throw new DispatchConflictException("Kuvvet geçersiz.");
        if (d.RankDefId.HasValue && !await db.RankDefs.AnyAsync(x=>x.Id==d.RankDefId && x.IsActive,ct)) throw new DispatchConflictException("Rütbe geçersiz.");
        if (d.MilitaryClassDefId.HasValue && !await db.MilitaryClassDefs.AnyAsync(x=>x.Id==d.MilitaryClassDefId && x.IsActive,ct)) throw new DispatchConflictException("Askerî sınıf geçersiz.");
        if (d.MilitaryUnitDefId.HasValue && !await db.MilitaryUnitDefs.AnyAsync(x=>x.Id==d.MilitaryUnitDefId && x.IsActive,ct)) throw new DispatchConflictException("Birlik geçersiz.");
        if (d.MilitarySchoolDefId.HasValue && !await db.MilitarySchoolDefs.AnyAsync(x=>x.Id==d.MilitarySchoolDefId && x.IsActive,ct)) throw new DispatchConflictException("Askerî okul geçersiz.");
        if (d.SenderMilitaryUnitDefId.HasValue && !await db.MilitaryUnitDefs.AnyAsync(x=>x.Id==d.SenderMilitaryUnitDefId && x.IsActive,ct)) throw new DispatchConflictException("Gönderen birlik geçersiz.");
        if (d.SenderMilitaryOfficeDefId.HasValue && !await db.MilitaryOfficeDefs.AnyAsync(x=>x.Id==d.SenderMilitaryOfficeDefId && x.IsActive,ct)) throw new DispatchConflictException("Gönderen askerlik şubesi geçersiz.");
        if (d.MilitaryOfficeDefId.HasValue && !await db.MilitaryOfficeDefs.AnyAsync(x=>x.Id==d.MilitaryOfficeDefId && x.IsActive,ct)) throw new DispatchConflictException("Askerlik şubesi geçersiz.");
        if (d.FlyingMissionDefId.HasValue && !await db.FlyingMissionDefs.AnyAsync(x=>x.Id==d.FlyingMissionDefId && x.IsActive,ct)) throw new DispatchConflictException("Uçucu görev geçersiz.");
        if (d.PlaneTypeDefId.HasValue && !await db.PlaneTypeDefs.AnyAsync(x=>x.Id==d.PlaneTypeDefId && x.IsActive,ct)) throw new DispatchConflictException("Uçak tipi geçersiz.");
    }

    /// <summary>Kişiye ait bilgileri eşleştirir; tarihsel sevk kaydını başka kimliğe bağlamaz.</summary>
    public static void AssignPerson(Person target,DispatchPersonInput p)
    {
        target.TcKimlikNo=p.TcKimlikNo?.Trim();
        target.YabanciKimlikNo=p.YabanciKimlikNo?.Trim();
        target.Name=p.Name.Trim();
        target.Surname=p.Surname.Trim();
        target.MotherName=p.MotherName?.Trim();
        target.FatherName=p.FatherName?.Trim();
        target.NationalityId=p.NationalityId;
        target.TownOfBirthId=p.TownOfBirthId;
        target.BirthDate=p.BirthDate;
        target.Gender=p.Gender;
        target.PhoneNumber=p.PhoneNumber?.Trim();
        target.Address=p.Address?.Trim();
        target.BirthPlaceText=p.BirthPlaceText?.Trim();
        target.BirthPlaceCode=p.BirthPlaceCode?.Trim();
    }

    /// <summary>Sevk alanlarının tek eşleştirme noktasını sağlar.</summary>
    public static void AssignDispatch(Dispatch target,DispatchDetailsInput d)
    {
        target.NthAdmission=d.NthAdmission;
        target.DispatchDocumentDate=d.DispatchDocumentDate;
        target.DispatchDocumentNo=d.DispatchDocumentNo?.Trim();
        target.ReasonForExaminationDefId=d.ReasonForExaminationDefId;
        target.DispatchScope=d.DispatchScope;
        target.DispatchScopeCityId=d.DispatchScopeCityId;
        target.DispatchScopeHospitalId=d.DispatchScopeHospitalId;
        target.HcAdmissionTypeDefId=d.HcAdmissionTypeDefId;
        target.ForcesCommandDefId=d.ForcesCommandDefId;
        target.RankDefId=d.RankDefId;
        target.MilitaryClassDefId=d.MilitaryClassDefId;
        target.MilitaryUnitDefId=d.MilitaryUnitDefId;
        target.MilitarySchoolDefId=d.MilitarySchoolDefId;
        target.SenderMilitaryUnitDefId=d.SenderMilitaryUnitDefId;
        target.SenderMilitaryOfficeDefId=d.SenderMilitaryOfficeDefId;
        target.SenderChairOther=d.SenderChairOther?.Trim();
        target.MilitaryOfficeDefId=d.MilitaryOfficeDefId;
        target.FlyingMissionDefId=d.FlyingMissionDefId;
        target.PlaneTypeDefId=d.PlaneTypeDefId;
        target.RegistryNumber=d.RegistryNumber?.Trim();
        target.LastNominationDate=d.LastNominationDate;
        target.MilitaryUnitDescription=d.MilitaryUnitDescription?.Trim();
        target.PurchaseId=d.PurchaseId;
        target.Description=d.Description?.Trim();
    }
}
