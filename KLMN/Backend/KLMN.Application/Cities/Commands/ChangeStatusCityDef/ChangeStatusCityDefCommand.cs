using MediatR;
using KLMN.Application.Cities.Responses;
namespace KLMN.Application.Cities.Commands.ChangeStatusCityDef;
public sealed record ChangeStatusCityDefCommand(Guid Id, uint Version, bool IsActive) : IRequest<CityDefMutationResponse>;
