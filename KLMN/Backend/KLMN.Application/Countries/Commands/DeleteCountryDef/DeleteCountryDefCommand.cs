using MediatR;

namespace KLMN.Application.Countries.Commands.DeleteCountryDef;
public sealed record DeleteCountryDefCommand(Guid Id, uint Version) : IRequest;
