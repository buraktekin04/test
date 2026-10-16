using KLMN.Application.MilitaryUnits.Responses;
using MediatR;

namespace KLMN.Application.MilitaryUnits.Queries.GetForceCommandOptions;

/// <summary>Birlik formu için varsayılan aktif/geçerli; kolon filtresi için silinmemiş tüm kuvvetler.</summary>
public sealed record GetForceCommandOptionsQuery(bool IncludeInactive = false)
    : IRequest<IReadOnlyList<ForceCommandOptionResponse>>;
