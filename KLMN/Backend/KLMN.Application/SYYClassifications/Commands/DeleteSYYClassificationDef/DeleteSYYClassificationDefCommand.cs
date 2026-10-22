using MediatR;

namespace KLMN.Application.SYYClassifications.Commands.DeleteSYYClassificationDef;
/// <summary>SYY Tanı Sınıflandırma Çizelgesi Delete CQRS komutu.</summary>
public sealed record DeleteSYYClassificationDefCommand(Guid Id,uint Version):IRequest;
