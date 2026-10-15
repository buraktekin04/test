using MediatR;

namespace KLMN.Application.MilitaryClassGroups.Queries.GetMilitaryClassGroupOptions;

/// <summary>Grup formunda kuvvetleri, düzenlemede seçilen pasif kuvveti de içerecek şekilde getirir.</summary>
public sealed record GetMilitaryClassGroupOptionsQuery(Guid? SelectedForceId = null)
    : IRequest<IReadOnlyList<MilitaryClassGroupForceOption>>;

/// <summary>Yetki vermeden yalnızca kuvvet adı ve kodunu taşıyan seçenek.</summary>
public sealed record MilitaryClassGroupForceOption(Guid Id, string Code, string Name);
