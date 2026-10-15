using KLMN.Application.MilitaryClassGroups.Commands.CreateMilitaryClassGroup;
using KLMN.Application.MilitaryClassGroups.Commands.UpdateMilitaryClassGroup;
using KLMN.Application.MilitaryClassGroups.Commands.ChangeStatusMilitaryClassGroup;
using KLMN.Application.MilitaryClassGroups.Commands.DeleteMilitaryClassGroup;
using KLMN.Application.MilitaryClassGroups.Queries.GetMilitaryClassGroups;
using KLMN.Application.MilitaryClassGroups.Responses;
using KLMN.Application.MilitaryClassGroupRelations.Commands.CreateMilitaryClassGroupRelation;
using KLMN.Application.MilitaryClassGroupRelations.Commands.UpdateMilitaryClassGroupRelation;
using KLMN.Application.MilitaryClassGroupRelations.Commands.ChangeStatusMilitaryClassGroupRelation;
using KLMN.Application.MilitaryClassGroupRelations.Commands.DeleteMilitaryClassGroupRelation;
using KLMN.Application.MilitaryClassGroupRelations.Queries.GetMilitaryClassGroupRelations;
using KLMN.Application.MilitaryClassGroupRelations.Responses;
using KLMN.Domain.Entities.MilitaryUnit;

/// <summary>İki yeni tanım/ilişki modülünün DB gerektirmeyen doğrulama regresyonları.</summary>
static void Check(bool ok, string name)
{
    if (!ok) throw new InvalidOperationException("FAIL: " + name);
    Console.WriteLine("PASS: " + name);
}
var force = Guid.NewGuid();
var militaryClass = Guid.NewGuid();
var group = Guid.NewGuid();
var start = new DateTime(2026, 10, 15, 0, 0, 0, DateTimeKind.Utc);
var g = new CreateMilitaryClassGroupCommand(force, "SINIF_GRUP", "Sınıf Grubu", "Grup", start, start.AddDays(1));
var groupValidator = new CreateMilitaryClassGroupCommandValidator();
Check(groupValidator.Validate(g).IsValid, "Grup oluşturma geçerli");
Check(!groupValidator.Validate(g with { ForcesCommandId = Guid.Empty }).IsValid, "Kuvvet gerekli");
Check(!groupValidator.Validate(g with { Code = "" }).IsValid, "Grup kodu gerekli");
Check(!groupValidator.Validate(g with { ValidTo = start }).IsValid, "Bitiş başlangıçtan sonra");
Check(!groupValidator.Validate(g with { ValidFrom = DateTime.SpecifyKind(start, DateTimeKind.Local) }).IsValid, "UTC gerekli");
Check(!new UpdateMilitaryClassGroupCommandValidator().Validate(
    new UpdateMilitaryClassGroupCommand(Guid.NewGuid(), 0, force, "SINIF_GRUP", "Grup", null, null, null)).IsValid,
    "Grup güncellemede xmin gerekir");
Check(!new ChangeStatusMilitaryClassGroupCommandValidator().Validate(
    new ChangeStatusMilitaryClassGroupCommand(Guid.Empty, 1, false)).IsValid, "Grup durum Id");
Check(!new DeleteMilitaryClassGroupCommandValidator().Validate(
    new DeleteMilitaryClassGroupCommand(Guid.NewGuid(), 0)).IsValid, "Grup silme xmin");
Check(!new GetMilitaryClassGroupsQueryValidator().Validate(new GetMilitaryClassGroupsQuery(PageSize:101)).IsValid,
    "Grup liste sayfa üst sınırı");

var relation = new CreateMilitaryClassGroupRelationCommand(force, militaryClass, group);
var relationValidator = new CreateMilitaryClassGroupRelationCommandValidator();
Check(relationValidator.Validate(relation).IsValid, "Üçlü eşleştirme geçerli");
Check(!relationValidator.Validate(relation with { ForcesCommandId = Guid.Empty }).IsValid, "Kuvvet zorunlu");
Check(!relationValidator.Validate(relation with { MilitaryClassId = Guid.Empty }).IsValid, "Askerî sınıf zorunlu");
Check(!relationValidator.Validate(relation with { MilitaryClassGroupId = Guid.Empty }).IsValid, "Grup zorunlu");
Check(!new UpdateMilitaryClassGroupRelationCommandValidator().Validate(
    new UpdateMilitaryClassGroupRelationCommand(Guid.NewGuid(), 0, force, militaryClass, group)).IsValid,
    "Eşleştirme güncellemede xmin gerekir");
Check(!new ChangeStatusMilitaryClassGroupRelationCommandValidator().Validate(
    new ChangeStatusMilitaryClassGroupRelationCommand(Guid.Empty, 1, true)).IsValid, "Eşleştirme durum Id");
Check(!new DeleteMilitaryClassGroupRelationCommandValidator().Validate(
    new DeleteMilitaryClassGroupRelationCommand(Guid.NewGuid(), 0)).IsValid, "Eşleştirme silme xmin");
Check(!new GetMilitaryClassGroupRelationsQueryValidator().Validate(
    new GetMilitaryClassGroupRelationsQuery(PageSize:101)).IsValid, "Eşleştirme liste sayfa üst sınırı");

var forceEntity = new ForcesCommandDef { Name = "Kara Kuvvetleri", Code = "K" };
var classEntity = new MilitaryClassDef { Name = "Piyade", Code = "PYD" };
var groupEntity = new MilitaryClassGroupDef {
    ForcesCommandId = forceEntity.Id, ForcesCommand = forceEntity,
    Code = "A", Name = "Grup A"
};
var groupDto = MilitaryClassGroupResponse.Projection.Compile()(groupEntity);
Check(groupDto.ForcesCommandName == "Kara Kuvvetleri", "Grup kuvvet projeksiyonu");
var item = new MilitaryClassGroupRelation {
    ForcesCommandId = forceEntity.Id, ForcesCommand = forceEntity,
    MilitaryClassId = classEntity.Id, MilitaryClass = classEntity,
    MilitaryClassGroupId = groupEntity.Id, MilitaryClassGroup = groupEntity
};
var relationDto = MilitaryClassGroupRelationResponse.Projection.Compile()(item);
Check(relationDto.MilitaryClassGroupName == "Grup A" && relationDto.MilitaryClassName == "Piyade",
    "Eşleştirme görünür ad projeksiyonu");
Console.WriteLine("DB gerektirmeyen regresyon kontrol kaynakları tamam.");
