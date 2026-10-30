using KLMN.Application.Dispatches.Models;
namespace KLMN.Application.Dispatches.Responses;
/// <summary>Yerel eşleşme bulunduğunda dönen kişi detayları.</summary>
public sealed record DispatchPersonResponse(DispatchPersonInput Person);
