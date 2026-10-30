namespace KLMN.Application.Dispatches.Responses;

/// <summary>
/// Nüfus kişi yanıtındaki varsa askerî/başvuru tanımlarının KLMN ID'lerine çevrilmiş hali.
/// Harici kaynak kodlarını doğrudan GUID kabul etmeyin; kurum servisinde eşleştirin.
/// </summary>
public sealed class DispatchRegistryMilitaryResponse
{
    public Guid? ForcesCommandDefId { get; set; }
    public Guid? MilitaryClassDefId { get; set; }
    public Guid? RankDefId { get; set; }
    public Guid? MilitaryUnitDefId { get; set; }
    public string? MilitaryUnitDescription { get; set; }
    public Guid? MilitarySchoolDefId { get; set; }
    public string? RegistryNumber { get; set; }
    public DateOnly? LastNominationDate { get; set; }
    public Guid? MilitaryOfficeDefId { get; set; }
    public Guid? FlyingMissionDefId { get; set; }
    public Guid? PlaneTypeDefId { get; set; }
    public Guid? HcAdmissionTypeDefId { get; set; }
}
