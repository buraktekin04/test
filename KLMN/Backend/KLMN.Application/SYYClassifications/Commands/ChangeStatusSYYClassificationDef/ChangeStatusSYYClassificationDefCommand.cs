using MediatR;
using KLMN.Application.SYYClassifications.Responses;
namespace KLMN.Application.SYYClassifications.Commands.ChangeStatusSYYClassificationDef;
/// <summary>SYY Tanı Sınıflandırma Çizelgesi ChangeStatus CQRS komutu.</summary>
public sealed record ChangeStatusSYYClassificationDefCommand(Guid Id,uint Version,bool IsActive):IRequest<SYYClassificationDefMutationResponse>;
