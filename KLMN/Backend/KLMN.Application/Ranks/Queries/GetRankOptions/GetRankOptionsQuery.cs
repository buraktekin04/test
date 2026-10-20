using MediatR;

namespace KLMN.Application.Ranks.Queries.GetRankOptions;

/// <summary>Rütbe grubu seçimleri; ayrı RankGroups izni gerektirmez.</summary>
public sealed record GetRankOptionsQuery(Guid? SelectedRankGroupId = null)
    : IRequest<IReadOnlyList<RankOptionResponse>>;

/// <summary>Dropdown rütbe grubu kimliği, kodu ve adı.</summary>
public sealed record RankOptionResponse(Guid Id, string Code, string Name);
