using KLMN.Application.Dispatches.Responses;
using MediatR;
namespace KLMN.Application.Dispatches.Queries.GetDispatchOptions;
/// <summary>Mevcut tanım tabloları için yazarken aranan, sınırlandırılmış seçimler.</summary>
public sealed record GetDispatchOptionsQuery(string Type,string? Search=null,Guid? SelectedId=null)
    :IRequest<IReadOnlyList<DispatchOption>>;
