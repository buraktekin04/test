using KLMN.Application.Dispatches.Responses;
using MediatR;
namespace KLMN.Application.Dispatches.Queries.LookupDispatchPerson;
/// <summary>Yalnız yerel kişiyi kimlik numarası ve doğum tarihi eşleşirse bulur; harici kimlik servis çağrısı yapmaz.</summary>
public sealed record LookupDispatchPersonQuery(string? TcKimlikNo,string? YabanciKimlikNo,DateOnly BirthDate)
    :IRequest<DispatchPersonResponse?>;
