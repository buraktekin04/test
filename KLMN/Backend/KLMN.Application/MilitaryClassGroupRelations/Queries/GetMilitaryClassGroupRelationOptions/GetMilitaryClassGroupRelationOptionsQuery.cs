using MediatR;

namespace KLMN.Application.MilitaryClassGroupRelations.Queries.GetMilitaryClassGroupRelationOptions;

/// <summary>Kuvvet bazlı eşleştirme formunu besleyen üç seçim listesi.</summary>
public sealed record GetMilitaryClassGroupRelationOptionsQuery(
    Guid? ForcesCommandId = null, Guid? SelectedForceId = null,
    Guid? SelectedClassId = null, Guid? SelectedGroupId = null) : IRequest<MilitaryClassGroupRelationOptions>;

/// <summary>Dropdown öğesinde yalnızca kimlik, kod ve ad gösterilir.</summary>
public sealed record MilitaryClassGroupRelationOption(Guid Id, string Code, string Name);

/// <summary>Seçili kuvvete göre filtrelenmiş grup listesi ve askerî sınıflar.</summary>
public sealed record MilitaryClassGroupRelationOptions(
    IReadOnlyList<MilitaryClassGroupRelationOption> ForcesCommands,
    IReadOnlyList<MilitaryClassGroupRelationOption> MilitaryClasses,
    IReadOnlyList<MilitaryClassGroupRelationOption> Groups);
