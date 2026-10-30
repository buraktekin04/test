using KLMN.Application.Dispatches.Responses;
using MediatR;
namespace KLMN.Application.Dispatches.Commands.UploadDispatchFile;
/// <summary>Sunucuda doğrulanmış en çok 5 MiB boyutundaki ekin kaydı.</summary>
public sealed record UploadDispatchFileCommand(Guid DispatchId,string FileName,string ContentType,byte[] Content)
    :IRequest<DispatchFileResponse>;
