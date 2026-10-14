using MediatR;
using KLMN.Application.Countries.Responses;
namespace KLMN.Application.Countries.Commands.ChangeStatusCountryDef;
public sealed record ChangeStatusCountryDefCommand(Guid Id, uint Version, bool IsActive) : IRequest<CountryDefMutationResponse>;
