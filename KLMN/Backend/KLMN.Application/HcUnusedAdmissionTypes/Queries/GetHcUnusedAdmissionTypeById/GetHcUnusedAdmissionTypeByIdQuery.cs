using KLMN.Application.HcUnusedAdmissionTypes.Responses;

using MediatR;
namespace KLMN.Application.HcUnusedAdmissionTypes.Queries.GetHcUnusedAdmissionTypeById;
/// <summary>Kullanılmayacak Başvuru Grupları: tek kayıt sorgusu.</summary>
public sealed record GetHcUnusedAdmissionTypeByIdQuery(Guid Id):IRequest<HcUnusedAdmissionTypeResponse>;
