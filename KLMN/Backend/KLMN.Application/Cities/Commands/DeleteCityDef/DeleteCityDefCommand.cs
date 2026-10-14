using MediatR;

namespace KLMN.Application.Cities.Commands.DeleteCityDef;
public sealed record DeleteCityDefCommand(Guid Id, uint Version) : IRequest;
