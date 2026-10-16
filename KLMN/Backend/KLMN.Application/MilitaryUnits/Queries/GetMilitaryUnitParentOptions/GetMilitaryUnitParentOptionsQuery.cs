using KLMN.Application.MilitaryUnits.Responses;
using MediatR;

namespace KLMN.Application.MilitaryUnits.Queries.GetMilitaryUnitParentOptions;

/// <summary>Formda aktif üst birlikleri, kolon filtresinde pasif dâhil silinmemiş birlikleri getirir.</summary>
public sealed record GetMilitaryUnitParentOptionsQuery(bool IncludeInactive = false)
    : IRequest<IReadOnlyList<MilitaryUnitParentOptionResponse>>;
