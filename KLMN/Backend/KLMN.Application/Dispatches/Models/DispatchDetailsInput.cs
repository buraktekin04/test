namespace KLMN.Application.Dispatches.Models;

/// <summary>Eski Dispatch ekranının sevk, başvuru ve askerî alanlarının yeni form sözleşmesi.</summary>
public sealed class DispatchDetailsInput
{
    public int? NthAdmission { get; set; }
    public DateOnly? DispatchDocumentDate { get; set; }
    public string? DispatchDocumentNo { get; set; }
    public Guid ReasonForExaminationDefId { get; set; }
    public int DispatchScope { get; set; } = 1;
    public Guid? DispatchScopeCityId { get; set; }
    public Guid? DispatchScopeHospitalId { get; set; }
    public Guid HcAdmissionTypeDefId { get; set; }
    public Guid? ForcesCommandDefId { get; set; }
    public Guid? RankDefId { get; set; }
    public Guid? MilitaryClassDefId { get; set; }
    public Guid? MilitaryUnitDefId { get; set; }
    public Guid? MilitarySchoolDefId { get; set; }
    public Guid? SenderMilitaryUnitDefId { get; set; }
    public Guid? SenderMilitaryOfficeDefId { get; set; }
    public string? SenderChairOther { get; set; }
    public Guid? MilitaryOfficeDefId { get; set; }
    public Guid? FlyingMissionDefId { get; set; }
    public Guid? PlaneTypeDefId { get; set; }
    public string? RegistryNumber { get; set; }
    public DateOnly? LastNominationDate { get; set; }
    public string? MilitaryUnitDescription { get; set; }
    public int? PurchaseId { get; set; }
    public string? Description { get; set; }
}
